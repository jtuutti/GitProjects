using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Mvc;
using SimpleViewEngine.Properties;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents an HTML view for the <see cref="HtmlViewEngine"/>.
    /// </summary>
    public class HtmlView : IView
    {
        private const string LayoutViewExtension = ".layout.html";
        private const string PartialViewExtension = ".partial.html";
        private const string ViewExtension = ".html";

        private static readonly Regex ReferenceDirectiveRegex = new Regex(@"^<!--\s*#layout\s+(.+)\s*=\s*\""(.+)\""\s*-->\s*",
                                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex IncludeFileDirectiveRegex = new Regex(@"<!--\s*#include\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex TitleRegex = new Regex(@"<title>.*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex TitleDirectiveRegex = new Regex(@"<!--\s*#title value=\""(.*)\""\s*-->",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex BodyDirectiveRegex = new Regex(@"<!--\s*#body\s*-->",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex LinkRegex = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
                                                            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string m_filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlView"/> class.
        /// </summary>
        /// <param name="filePath">The local view file path.</param>
        public HtmlView(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            m_filePath = filePath;
        }

        /// <summary>
        /// Renders the specified view context by using the specified the writer object.
        /// </summary>
        /// <param name="viewContext">The view context.</param><param name="writer">The writer object.</param>
        /// <exception cref="FileNotFoundException">If the view file is not found.</exception>
        /// <exception cref="InvalidOperationException">
        /// If the view does not have the correct file extension (.html, .partial.html or .layout.html).
        /// </exception>
        /// <exception cref="RecursiveViewReferenceException">
        /// If a partial view file references itself recursively.
        /// </exception>
        public virtual void Render(ViewContext viewContext, TextWriter writer)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (!File.Exists(m_filePath))
            {
                throw new FileNotFoundException(String.Format(Resources.ViewNotFound, m_filePath));
            }

            string html = ReadHtml(m_filePath, ViewType.View);

            Match titleMatch = TitleDirectiveRegex.Match(html);

            if (titleMatch.Success)
            {
                html = TitleDirectiveRegex.Replace(html, String.Empty);
            }

            Match layoutMatch = ReferenceDirectiveRegex.Match(html);

            if (layoutMatch.Success)
            {
                html = ParseLayout(layoutMatch, html);

                if (titleMatch.Success)
                {
                    html = TitleRegex.Replace(html, m => String.Concat("<title>", titleMatch.Groups[1].Value, "</title>"));
                }
            }

            string fileHtml = Parse(html, m_filePath);

            fileHtml = LinkRegex.Replace(fileHtml, m =>
            {
                string group1 = m.Result("$1"), group2 = m.Result("$2"), group3 = m.Result("$3"), group5 = m.Result("$5");
                string applicationPath = viewContext.HttpContext.Request.ApplicationPath != null ?
                                            viewContext.HttpContext.Request.ApplicationPath.TrimEnd('/') :
                                            String.Empty;

                return String.Concat(group1, group2, group3, applicationPath, group5);
            });

            writer.Write(fileHtml);
        }

        private static string ReadHtml(string filePath, ViewType viewType)
        {
            filePath = filePath.Trim(' ', '\t');

            switch (viewType)
            {
                case ViewType.PartialView:
                    if (!filePath.EndsWith(PartialViewExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(String.Format(Resources.InvalidPartialViewExtension, filePath));
                    }
                    break;
                case ViewType.Layout:
                    if (!filePath.EndsWith(LayoutViewExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(String.Format(Resources.InvalidLayoutViewExtension, filePath));
                    }
                    break;
                default:
                    if (!filePath.EndsWith(ViewExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(String.Format(Resources.InvalidViewExtension, filePath));
                    }
                    break;
            }

            string html = File.ReadAllText(filePath, Encoding.UTF8);

            return html.Trim(' ', '\n', '\r', '\t');
        }

        private static string GenerateLayout(string layoutFilePath, string html)
        {
            if (!File.Exists(layoutFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.LayoutViewNotFound, layoutFilePath));
            }

            string layoutHtml = ReadHtml(layoutFilePath, ViewType.Layout);
            layoutHtml = BodyDirectiveRegex.Replace(layoutHtml, html);

            return layoutHtml;
        }

        private string ParseLayout(Match layoutMatch, string html)
        {
            string layoutFileType = layoutMatch.Result("$1").Trim();
            string layoutFilePath;

            if (String.Equals("virtual", layoutFileType, StringComparison.OrdinalIgnoreCase))
            {
                layoutFilePath = MapPath(layoutMatch.Result("$2"));
            }
            else if (String.Equals("file", layoutFileType, StringComparison.OrdinalIgnoreCase))
            {
                layoutFilePath = layoutMatch.Result("$2").Trim();
            }
            else
            {
                return html;
            }

            html = ReferenceDirectiveRegex.Replace(html, String.Empty);
            html = GenerateLayout(layoutFilePath, html);

            return html;
        }

        private string MapPath(string viewPath)
        {
            if (viewPath.IndexOf('~') == 0)
            {
                return HostingEnvironment.MapPath(viewPath.Trim());
            }
            
            return m_filePath.Substring(0, m_filePath.LastIndexOf('\\') + 1) + viewPath.Trim();
        }

        private string Parse(string html, string masterFilePath)
        {
            return IncludeFileDirectiveRegex.Replace(html, match => GetMatch(match, masterFilePath));
        }

        private string GetMatch(Match match, string masterFilePath)
        {
            if (!match.Success)
            {
                return String.Empty;
            }

            string fileType = match.Result("$1").Trim();
            string filePath;

            if (String.Equals("virtual", fileType, StringComparison.OrdinalIgnoreCase))
            {
                filePath = MapPath(match.Result("$2"));
            }
            else if (String.Equals("file", fileType, StringComparison.OrdinalIgnoreCase))
            {
                filePath = match.Result("$2").Trim();
            }
            else
            {
                return match.Result("$0");
            }

            if (String.Equals(masterFilePath, filePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new RecursiveViewReferenceException(String.Format(Resources.RecursivePartialViewReference, filePath));
            }

            if (filePath == null || !File.Exists(filePath))
            {
                throw new FileNotFoundException(String.Format(Resources.PartialViewNotFound, filePath));
            }

            string html = ReadHtml(filePath, ViewType.PartialView);

            return Parse(html, filePath);
        }
    }
}
