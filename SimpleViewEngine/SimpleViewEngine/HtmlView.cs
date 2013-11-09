using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using SimpleViewEngine.Properties;
using SimpleViewEngine.Utilities;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents an HTML view for the <see cref="HtmlViewEngine"/>.
    /// </summary>
    public class HtmlView : IView
    {
        private const string FileValueType = "FILE";
        private const string LayoutViewExtension = ".layout.html";
        private const string LayoutViewLocation = "~/views/shared/{0}" + LayoutViewExtension;
        private const string NameValueType = "NAME";
        private const string PartialViewExtension = ".partial.html";
        private const string ReferencedFilePathKey = "_ReferencedFilePaths";
        private const string RenderedPartialFilePathKey = "_RenderedPartialFilePaths";
        private const string UrlValueType = "URL";
        private const string ViewExtension = ".html";
        private const string ViewKeyPrefix = "HTMLView_";

        private readonly string m_filePath;
        private readonly DateTime? m_cacheExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlView"/> class.
        /// </summary>
        /// <param name="filePath">The local view file path.</param>
        /// <param name="cacheExpiration">An HTML cache expiration time.</param>
        public HtmlView(string filePath, DateTime? cacheExpiration)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            m_filePath = filePath;
            m_cacheExpiration = cacheExpiration;
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


            bool isView = m_filePath.IndexOf(PartialViewExtension, StringComparison.OrdinalIgnoreCase) < 0;
            string cacheKey = null;

            if (m_cacheExpiration.HasValue && isView)
            {
                cacheKey = String.Concat(ViewKeyPrefix, m_filePath.ToUpperInvariant());

                var cachedHtml = HttpRuntime.Cache[cacheKey] as string;

                if (!String.IsNullOrEmpty(cachedHtml))
                {
                    writer.Write(GenerateModelScript(cachedHtml, viewContext.ViewData.Model));
                    return;
                }
            }

            GetReferencedFilePaths(viewContext).Add(m_filePath);

            string html = ReadViewFileHtml(m_filePath, ViewType.View);
            html = isView ? ParseViewEngineDirectives(viewContext, html) : RemoveViewEngineDirectives(html);

            string viewHtml = ParseViewContent(viewContext, html);

            viewHtml = RegularExpressions.Link.Replace(viewHtml, m =>
            {
                string group1 = m.Result("$1"), group2 = m.Result("$2"), group3 = m.Result("$3"), group5 = m.Result("$5");
                string applicationPath = viewContext.HttpContext.Request.ApplicationPath != null ?
                    viewContext.HttpContext.Request.ApplicationPath.TrimEnd('/') :
                    String.Empty;

                return String.Concat(group1, group2, group3, applicationPath, group5);
            });

            viewContext.HttpContext.Items[RenderedPartialFilePathKey] = null;

            if (cacheKey != null)
            {
                HttpRuntime.Cache.Add(cacheKey,
                                      viewHtml,
                                      new CacheDependency(GetReferencedFilePaths(viewContext).ToArray()),
                                      m_cacheExpiration.Value,
                                      Cache.NoSlidingExpiration,
                                      CacheItemPriority.Normal,
                                      null);
            }

            writer.Write(isView ? GenerateModelScript(viewHtml, viewContext.ViewData.Model) : viewHtml);
        }

        private static string GenerateModelScript(string html, object model)
        {
            Match modelMatch = RegularExpressions.ModelDirective.Match(html);

            if (!modelMatch.Success)
            {
                return html;
            }

            return RegularExpressions.ModelDirective.Replace(html, model != null ?
                                                                        ModelScriptTagCreator.Create(model) :
                                                                        String.Empty);
        }

        private static HashSet<string> GetReferencedFilePaths(ControllerContext context)
        {
            var filePaths = context.HttpContext.Items[ReferencedFilePathKey] as HashSet<string>;

            if (filePaths != null)
            {
                return filePaths;
            }

            filePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            context.HttpContext.Items[ReferencedFilePathKey] = filePaths;

            return filePaths;
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

        private static string ParseLayoutViewContent(ControllerContext context, string layoutFilePath, string html)
        {
            if (!File.Exists(layoutFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.LayoutViewNotFound, layoutFilePath));
            }

            GetReferencedFilePaths(context).Add(layoutFilePath);

            string layoutHtml = ReadViewFileHtml(layoutFilePath, ViewType.Layout);

            Match headHtmlMatch = RegularExpressions.HeadBodyDirective.Match(html);

            if (headHtmlMatch.Success)
            {
                string headHtml = headHtmlMatch.Groups[1].Value;

                html = RegularExpressions.HeadBodyDirective.Replace(html, String.Empty);
                layoutHtml = RegularExpressions.HeadDirective.Replace(layoutHtml, headHtml.TrimLine());
            }
            else
            {
                layoutHtml = RegularExpressions.HeadDirective.Replace(layoutHtml, String.Empty);
            }

            Match scriptsHtmlMatch = RegularExpressions.ScriptsBodyDirective.Match(html);

            if (scriptsHtmlMatch.Success)
            {
                string scriptsHtml = scriptsHtmlMatch.Groups[1].Value;

                html = RegularExpressions.ScriptsBodyDirective.Replace(html, String.Empty);
                layoutHtml = RegularExpressions.ScriptsDirective.Replace(layoutHtml, scriptsHtml.TrimLine());
            }
            else
            {
                layoutHtml = RegularExpressions.ScriptsDirective.Replace(layoutHtml, String.Empty);
            }

            layoutHtml = RegularExpressions.BodyDirective.Replace(layoutHtml, html.TrimLine());

            return layoutHtml;
        }

        private static string ReadViewFileHtml(string filePath, ViewType viewType)
        {
            filePath = filePath.TrimLine();

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

            return File.ReadAllText(filePath, Encoding.UTF8).TrimLine();
        }
        
        private static string RemoveViewEngineDirectives(string html)
        {
            Match titleMatch = RegularExpressions.TitleDirective.Match(html);

            if (titleMatch.Success)
            {
                html = RegularExpressions.TitleDirective.Replace(html, String.Empty);
            }

            Match layoutMatch = RegularExpressions.ReferenceDirective.Match(html);

            if (layoutMatch.Success)
            {
                html = RegularExpressions.ReferenceDirective.Replace(html, String.Empty);
            }

            Match headMatch = RegularExpressions.HeadBodyDirective.Match(html);

            if (headMatch.Success)
            {
                html = RegularExpressions.HeadBodyDirective.Replace(html, String.Empty);
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
                    GetReferencedFilePaths(context).Add(htmlView.m_filePath);

                    string upperCaseFilePath = htmlView.m_filePath.ToUpperInvariant();

                    partialViewStack = GetRenderedPartialFilePaths(context);

                    if (partialViewStack.Contains(upperCaseFilePath))
                    {
                        throw new RecursiveViewReferenceException(String.Format(Resources.RecursivePartialViewReference, htmlView.m_filePath));
                    }

                    partialViewStack.Push(upperCaseFilePath);
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
            string layoutFileType = layoutMatch.Result("$1").Trim().ToUpperInvariant(),
                   layoutValue = layoutMatch.Result("$2"),
                   layoutFilePath;

            switch (layoutFileType)
            {
                case UrlValueType:
                    layoutFilePath = MapPath(context, layoutValue);
                    break;
                case FileValueType:
                    layoutFilePath = layoutValue.Trim();
                    break;
                case NameValueType:
                    layoutFilePath = MapPath(context, String.Format(CultureInfo.InvariantCulture, LayoutViewLocation, layoutValue.Trim()));
                    break;
                default:
                    return html;
            }

            html = RegularExpressions.ReferenceDirective.Replace(html, String.Empty);
            html = ParseLayoutViewContent(context, layoutFilePath, html);

            return html;
        }

        private string ParsePartialView(ControllerContext context, Match match)
        {
            if (!match.Success)
            {
                return String.Empty;
            }

            string partialViewFileType = match.Result("$1").Trim().ToUpperInvariant(),
                   partialViewValue = match.Result("$2").Trim(),
                   partialViewFilePath;

            switch (partialViewFileType)
            {
                case UrlValueType:
                    partialViewFilePath = MapPath(context, partialViewValue);
                    break;
                case FileValueType:
                    partialViewFilePath = partialViewValue;
                    break;
                case NameValueType:
                    return RenderPartialView(context, partialViewValue);
                default:
                    return match.Result("$0");
            }

            if (partialViewFilePath == null || !File.Exists(partialViewFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.PartialViewNotFound, partialViewFilePath));
            }

            Stack<string> partialViewStack = GetRenderedPartialFilePaths(context);

            if (partialViewStack.Contains(partialViewFilePath.ToUpperInvariant()))
            {
                throw new RecursiveViewReferenceException(String.Format(Resources.RecursivePartialViewReference, partialViewFilePath));
            }

            GetReferencedFilePaths(context).Add(partialViewFilePath);
            partialViewStack.Push(partialViewFilePath.ToUpperInvariant());

            string html = ReadViewFileHtml(partialViewFilePath, ViewType.PartialView);
            html = ParseViewContent(context, html);

            partialViewStack.Pop();

            return html;
        }

        private string ParseViewContent(ControllerContext context, string html)
        {
            return RegularExpressions.PartialViewDirective.Replace(html, match => ParsePartialView(context, match));
        }

        private string ParseViewEngineDirectives(ControllerContext context, string html)
        {
            Match titleMatch = RegularExpressions.TitleDirective.Match(html);

            if (titleMatch.Success)
            {
                html = RegularExpressions.TitleDirective.Replace(html, String.Empty);
            }

            Match layoutMatch = RegularExpressions.ReferenceDirective.Match(html);

            if (layoutMatch.Success)
            {
                html = ParseLayoutView(context, layoutMatch, html);

                if (titleMatch.Success)
                {
                    html = RegularExpressions.Title.Replace(html, m => String.Concat("<title>", titleMatch.Groups[1].Value, "</title>"));
                }
            }

            return html;
        }
    }
}
