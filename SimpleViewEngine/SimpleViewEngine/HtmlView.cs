using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Helpers;
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
        private const string LayoutViewLocation = "~/views/{0}" + LayoutViewExtension;
        private const string PartialViewExtension = ".partial.html";
        private const string RenderedPartialFilePathKey = "_RenderedPartialFilePaths";
        private const string ViewExtension = ".html";

        private static readonly Regex referenceDirectiveRegex = new Regex(@"<!--\s*#layout\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex partialViewDirectiveRegex = new Regex(@"<!--\s*#partial\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex titleRegex = new Regex(@"<title>.*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex titleDirectiveRegex = new Regex(@"<!--\s*#title text=\""(.*)\""\s*-->",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex headDirectiveRegex = new Regex(@"<!--\s*#head\s*-->",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex headBodyDirectiveRegex = new Regex(@"<!--\s*#head(.*)/#head\s*-->",
                                                                         RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex scriptsDirectiveRegex = new Regex(@"<!--\s*#scripts\s*-->",
                                                                        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex scriptsBodyDirectiveRegex = new Regex(@"<!--\s*#scripts(.*)/#scripts\s*-->",
                                                                            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex modelDirectiveRegex = new Regex(@"<!--\s*#model\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex bodyDirectiveRegex = new Regex(@"<!--\s*#body\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex linkRegex = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
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

            string html = ReadViewFileHtml(m_filePath, ViewType.View);

            if (m_filePath.IndexOf(PartialViewExtension, StringComparison.OrdinalIgnoreCase) < 0)
            {
                html = GenerateModelScript(ParseViewEngineDirectives(viewContext, html), viewContext.ViewData.Model);
            }
            else
            {
                html = RemoveViewEngineDirectives(html);
            }

            string fileHtml = ParseViewContent(viewContext, html);

            fileHtml = linkRegex.Replace(fileHtml, m =>
            {
                string group1 = m.Result("$1"), group2 = m.Result("$2"), group3 = m.Result("$3"), group5 = m.Result("$5");
                string applicationPath = viewContext.HttpContext.Request.ApplicationPath != null ?
                                            viewContext.HttpContext.Request.ApplicationPath.TrimEnd('/') :
                                            String.Empty;

                return String.Concat(group1, group2, group3, applicationPath, group5);
            });

            writer.Write(fileHtml);

            viewContext.HttpContext.Items[RenderedPartialFilePathKey] = null;
        }

        private static string GenerateModelScript(string html, object model)
        {
            Match modelMatch = modelDirectiveRegex.Match(html);

            if (!modelMatch.Success)
            {
                return html;
            }

            if (model != null)
            {
                string serializedModel = Json.Encode(model);

                var modelBuilder = new StringBuilder();
                modelBuilder.AppendLine("<script type=\"text/javascript\">");
                modelBuilder.Append("window.Model = ").Append(serializedModel).AppendLine(";");
                modelBuilder.AppendLine("</script>");

                html = modelDirectiveRegex.Replace(html, TrimLine(modelBuilder.ToString()));
            }
            else
            {
                html = modelDirectiveRegex.Replace(html, String.Empty);
            }

            return html;
        }

        private static Stack<string> GetRenderedPartialFilePaths(ControllerContext context)
        {
            var filePaths = context.HttpContext.Items[RenderedPartialFilePathKey] as Stack<string>;

            if (filePaths != null)
            {
                return filePaths;
            }

            filePaths = new Stack<string>();
            context.HttpContext.Items[RenderedPartialFilePathKey] = filePaths;

            return filePaths;
        }

        private static string ParseLayoutViewContent(string layoutFilePath, string html)
        {
            if (!File.Exists(layoutFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.LayoutViewNotFound, layoutFilePath));
            }

            string layoutHtml = ReadViewFileHtml(layoutFilePath, ViewType.Layout);

            Match headHtmlMatch = headBodyDirectiveRegex.Match(html);

            if (headHtmlMatch.Success)
            {
                string headHtml = headHtmlMatch.Groups[1].Value;

                html = headBodyDirectiveRegex.Replace(html, String.Empty);
                layoutHtml = headDirectiveRegex.Replace(layoutHtml, TrimLine(headHtml));
            }
            else
            {
                layoutHtml = headDirectiveRegex.Replace(layoutHtml, String.Empty);
            }

            Match scriptsHtmlMatch = scriptsBodyDirectiveRegex.Match(html);

            if (scriptsHtmlMatch.Success)
            {
                string scriptsHtml = scriptsHtmlMatch.Groups[1].Value;

                html = scriptsBodyDirectiveRegex.Replace(html, String.Empty);
                layoutHtml = scriptsDirectiveRegex.Replace(layoutHtml, TrimLine(scriptsHtml));
            }
            else
            {
                layoutHtml = scriptsDirectiveRegex.Replace(layoutHtml, String.Empty);
            }

            layoutHtml = bodyDirectiveRegex.Replace(layoutHtml, TrimLine(html));

            return layoutHtml;
        }

        private static string ReadViewFileHtml(string filePath, ViewType viewType)
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

            return TrimLine(html);
        }
        
        private static string RemoveViewEngineDirectives(string html)
        {
            Match titleMatch = titleDirectiveRegex.Match(html);

            if (titleMatch.Success)
            {
                html = titleDirectiveRegex.Replace(html, String.Empty);
            }

            Match layoutMatch = referenceDirectiveRegex.Match(html);

            if (layoutMatch.Success)
            {
                html = referenceDirectiveRegex.Replace(html, String.Empty);
            }

            Match headMatch = headBodyDirectiveRegex.Match(html);

            if (headMatch.Success)
            {
                html = headBodyDirectiveRegex.Replace(html, String.Empty);
            }

            return html;
        }

        private static string RenderPartialView(ControllerContext context, string viewName)
        {
            if (String.IsNullOrEmpty(viewName))
            {
                return String.Empty;
            }

            using (var writer = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);

                if (viewResult == null || viewResult.View == null)
                {
                    throw new FileNotFoundException(String.Format(Resources.PartialViewNotFound, viewName));
                }

                var htmlView = viewResult.View as HtmlView;
                Stack<string> partialViewStack = null;

                if (htmlView != null && htmlView.m_filePath != null)
                {
                    partialViewStack = GetRenderedPartialFilePaths(context);

                    if (partialViewStack.Contains(htmlView.m_filePath.ToUpperInvariant()))
                    {
                        throw new RecursiveViewReferenceException(String.Format(Resources.RecursivePartialViewReference, htmlView.m_filePath));
                    }

                    partialViewStack.Push(htmlView.m_filePath.ToUpperInvariant());
                }

                var viewContext = new ViewContext(context, viewResult.View, new ViewDataDictionary(), new TempDataDictionary(), writer);
                viewResult.View.Render(viewContext, writer);

                if (partialViewStack != null)
                {
                    partialViewStack.Pop();
                }

                return writer.GetStringBuilder().ToString();
            }
        }

        private static string TrimLine(string line)
        {
            return line.Trim(' ', '\n', '\r', '\t');
        }

        private string MapPath(ControllerContext context, string viewPath)
        {
            if (viewPath.IndexOf('~') == 0)
            {
                return context.HttpContext.Server.MapPath(viewPath.Trim());
            }
            
            return m_filePath.Substring(0, m_filePath.LastIndexOf('\\') + 1) + viewPath.Trim();
        }

        private string ParseLayoutView(ControllerContext context, Match layoutMatch, string html)
        {
            string layoutFileType = layoutMatch.Result("$1").Trim().ToUpperInvariant(), layoutFilePath;
            
            switch (layoutFileType)
            {
                case "URL":
                    layoutFilePath = MapPath(context, layoutMatch.Result("$2"));
                    break;
                case "FILE":
                    layoutFilePath = layoutMatch.Result("$2").Trim();
                    break;
                case "NAME":
                    layoutFilePath = MapPath(context, String.Format(CultureInfo.InvariantCulture, LayoutViewLocation, layoutMatch.Result("$2").Trim()));
                    break;
                default:
                    return html;
            }

            html = referenceDirectiveRegex.Replace(html, String.Empty);
            html = ParseLayoutViewContent(layoutFilePath, html);

            return html;
        }

        private string ParsePartialView(ControllerContext context, Match match)
        {
            if (!match.Success)
            {
                return String.Empty;
            }

            string fileType = match.Result("$1").Trim().ToUpperInvariant(), filePath;

            switch (fileType)
            {
                case "URL":
                    filePath = MapPath(context, match.Result("$2").Trim());
                    break;
                case "FILE":
                    filePath = match.Result("$2").Trim();
                    break;
                case "NAME":
                    return RenderPartialView(context, match.Result("$2").Trim());
                default:
                    return match.Result("$0");
            }

            if (filePath == null || !File.Exists(filePath))
            {
                throw new FileNotFoundException(String.Format(Resources.PartialViewNotFound, filePath));
            }

            Stack<string> partialViewStack = GetRenderedPartialFilePaths(context);

            if (partialViewStack.Contains(filePath.ToUpperInvariant()))
            {
                throw new RecursiveViewReferenceException(String.Format(Resources.RecursivePartialViewReference, filePath));
            }

            partialViewStack.Push(filePath.ToUpperInvariant());

            string html = ReadViewFileHtml(filePath, ViewType.PartialView);
            html = ParseViewContent(context, html);

            partialViewStack.Pop();

            return html;
        }

        private string ParseViewContent(ControllerContext context, string html)
        {
            return partialViewDirectiveRegex.Replace(html, match => ParsePartialView(context, match));
        }

        private string ParseViewEngineDirectives(ControllerContext context, string html)
        {
            Match titleMatch = titleDirectiveRegex.Match(html);

            if (titleMatch.Success)
            {
                html = titleDirectiveRegex.Replace(html, String.Empty);
            }

            Match layoutMatch = referenceDirectiveRegex.Match(html);

            if (layoutMatch.Success)
            {
                html = ParseLayoutView(context, layoutMatch, html);

                if (titleMatch.Success)
                {
                    html = titleRegex.Replace(html, m => String.Concat("<title>", titleMatch.Groups[1].Value, "</title>"));
                }
            }

            return html;
        }
    }
}
