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
using System.Web.Optimization;
using SimpleViewEngine.Properties;
using SimpleViewEngine.Serializer;
using SimpleViewEngine.Utilities;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents an HTML view for the <see cref="HtmlViewEngine"/>.
    /// </summary>
    public class HtmlView : IView, IViewDataContainer
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

        private readonly IModelSerializer m_serializer;
        private readonly string m_filePath;
        private readonly string m_version;
        private readonly bool m_antiForgeryTokenSupport;
        private readonly bool m_bundleSupport;
        private readonly DateTime? m_cacheExpiration;
        private readonly string m_modelPropertyName;
        private readonly bool m_minifyHtml;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlView"/> class.
        /// </summary>
        /// <param name="serializer">The model serializer.</param>
        /// <param name="filePath">The local view file path.</param>
        /// <param name="version">An optional application version.</param>
        /// <param name="antiForgeryTokenSupport">
        /// A value indicating whether anti-forgery tokens are supported.
        /// </param>
        /// <param name="bundleSupport">
        /// A value indicating whether MVC bundles tokens are supported.
        /// </param>
        /// <param name="cacheExpiration">An optional HTML cache expiration time.</param>
        /// <param name="modelPropertyName">
        /// An optional model property name returned from the controller.
        /// </param>
        /// <param name="minifyHtml">
        /// A value indicating whether the output HTML should be minified.
        /// </param>
        public HtmlView(IModelSerializer serializer,
                        string filePath,
                        string version,
                        bool antiForgeryTokenSupport,
                        bool bundleSupport,
                        DateTime? cacheExpiration,
                        string modelPropertyName,
                        bool minifyHtml)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            m_serializer = serializer;
            m_filePath = filePath;
            m_version = version;
            m_antiForgeryTokenSupport = antiForgeryTokenSupport;
            m_bundleSupport = bundleSupport;
            m_cacheExpiration = cacheExpiration;
            m_minifyHtml = minifyHtml;

            if (!String.IsNullOrWhiteSpace(modelPropertyName))
            {
                m_modelPropertyName = modelPropertyName.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the view data dictionary. This property is not supported.
        /// </summary>
        /// <returns>
        /// The view data dictionary.
        /// </returns>
        public ViewDataDictionary ViewData
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
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
                    writer.Write(PostRender(viewContext, cachedHtml, viewContext.ViewData.Model));
                    return;
                }
            }

            GetReferencedFilePaths(viewContext).Add(m_filePath);

            string html = ReadViewHtml(m_filePath, ViewType.View);

            if (isView)
            {
                html = GenerateView(viewContext, html);
            }

            html = ParseViewContent(viewContext, html);

            viewContext.HttpContext.Items[RenderedPartialFilePathKey] = null;

            if (cacheKey != null)
            {
                if (m_minifyHtml)
                {
                    html = MinifyHtml(html);
                }

                HttpRuntime.Cache.Add(cacheKey,
                                      html,
                                      new CacheDependency(GetReferencedFilePaths(viewContext).ToArray()),
                                      m_cacheExpiration.Value,
                                      Cache.NoSlidingExpiration,
                                      CacheItemPriority.Normal,
                                      null);
            }

            writer.Write(isView ? PostRender(viewContext, html, viewContext.ViewData.Model) : html);
        }

        private static HashSet<string> GetReferencedFilePaths(ViewContext context)
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

        private static Stack<string> GetRenderedPartialFilePaths(ViewContext context)
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

        private static string ReadViewHtml(string filePath, ViewType viewType)
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

        private static string GeneratePartialView(ViewContext context, string viewName)
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

        private static string TransformBundles(string html)
        {
            MatchCollection cssBundleMatches = RegularExpressions.CssBundleServerTag.Matches(html);

            foreach (Match cssBundleMatch in cssBundleMatches)
            {
                string bundleUrl = cssBundleMatch.Groups[1].Value;

                if (String.IsNullOrEmpty(bundleUrl))
                {
                    continue;
                }

                html = html.Replace(cssBundleMatch.Value, Styles.Render(bundleUrl.Trim()).ToHtmlString().TrimLine());
            }

            MatchCollection scriptBundleMatches = RegularExpressions.ScriptBundleServerTag.Matches(html);

            foreach (Match scriptBundleMatch in scriptBundleMatches)
            {
                string bundleUrl = scriptBundleMatch.Groups[1].Value;

                if (String.IsNullOrEmpty(bundleUrl))
                {
                    continue;
                }

                html = html.Replace(scriptBundleMatch.Value, Scripts.Render(bundleUrl.Trim()).ToHtmlString().TrimLine());
            }

            return html;
        }

        private static string MinifyHtml(string html)
        {
            Match doNotMinify = RegularExpressions.DoNotMinifyTag.Match(html);

            if (doNotMinify.Success)
            {
                html = html.Replace(doNotMinify.Value, String.Empty).TrimLine();
                return html;
            }

            html = RegularExpressions.WhiteSpaceBetweenTags.Replace(html, ">");
            html = RegularExpressions.WhiteSpaceBetweenLines.Replace(html, "<");

            return html;
        }

        private string GenerateView(ViewContext context, string html)
        {
            Match baseMatch = RegularExpressions.BaseServerTag.Match(html);

            if (baseMatch.Success)
            {
                html = RegularExpressions.BaseServerTag.Replace(html, String.Empty);
            }

            Match titleMatch = RegularExpressions.TitleServerTag.Match(html);

            if (titleMatch.Success)
            {
                html = RegularExpressions.TitleServerTag.Replace(html, String.Empty);
            }

            Match layoutMatch = RegularExpressions.LayoutServerTag.Match(html);

            if (layoutMatch.Success)
            {
                html = ParseLayoutViewContent(context, layoutMatch, html);

                if (baseMatch.Success)
                {
                    html = RegularExpressions.BaseTag.Replace(html, m => String.Format(CultureInfo.InvariantCulture, "<base{0}>", baseMatch.Groups[1].Value));
                }

                if (titleMatch.Success)
                {
                    html = RegularExpressions.TitleTag.Replace(html, m => String.Concat("<title>", titleMatch.Groups[1].Value, "</title>"));
                }
            }

            Match namespaceMatch = RegularExpressions.NamespaceAttribute.Match(html);

            if (namespaceMatch.Success)
            {
                html = html.Replace(namespaceMatch.Groups[1].Value, String.Empty);
            }

            return html;
        }

        private string ParseLayoutViewContent(ViewContext context, Match layoutMatch, string html)
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

            html = RegularExpressions.LayoutServerTag.Replace(html, String.Empty);

            if (!File.Exists(layoutFilePath))
            {
                throw new FileNotFoundException(String.Format(Resources.LayoutViewNotFound, layoutFilePath));
            }

            GetReferencedFilePaths(context).Add(layoutFilePath);

            string layoutHtml = ReadViewHtml(layoutFilePath, ViewType.Layout);

            MatchCollection sectionMatches = RegularExpressions.SectionServerTag.Matches(layoutHtml);

            foreach (Match sectionMatch in sectionMatches)
            {
                var sectionTagRegex = new Regex(String.Format(CultureInfo.InvariantCulture, RegularExpressions.CustomSectionTagTemplate, sectionMatch.Groups[1].Value),
                                                RegexOptions.IgnoreCase | RegexOptions.Singleline);

                Match sectionTagMatch = sectionTagRegex.Match(html);
                
                layoutHtml = layoutHtml.Replace(sectionMatch.Value, sectionTagMatch.Success ? sectionTagMatch.Groups[1].Value.TrimLine() : String.Empty);
                html = sectionTagRegex.Replace(html, String.Empty);
            }

            layoutHtml = RegularExpressions.BodyServerTag.Replace(layoutHtml, html.TrimLine());

            return layoutHtml;
        }

        private string ParseViewContent(ViewContext context, string html)
        {
            html = RegularExpressions.PartialServerTag.Replace(html, match => ParsePartialViewContent(context, match).TrimLine());

            if (m_bundleSupport)
            {
                html = TransformBundles(html);
            }

            MatchCollection debugHtmlMatches = RegularExpressions.DebugServerTag.Matches(html);

            foreach (Match debugHtmlMatch in debugHtmlMatches)
            {
                string debugHtml = debugHtmlMatch.Groups[1].Value;

                if (context.HttpContext.IsDebuggingEnabled)
                {
                    html = html.Replace(debugHtmlMatch.Value, debugHtml.TrimLine());
                }
                else
                {
                    html = html.Replace(debugHtmlMatch.Value, String.Empty);
                }
            }

            MatchCollection releaseHtmlMatches = RegularExpressions.ReleaseServerTag.Matches(html);

            foreach (Match releaseHtmlMatch in releaseHtmlMatches)
            {
                string releaseHtml = releaseHtmlMatch.Groups[1].Value;

                if (!context.HttpContext.IsDebuggingEnabled)
                {
                    html = html.Replace(releaseHtmlMatch.Value, releaseHtml.TrimLine());
                }
                else
                {
                    html = html.Replace(releaseHtmlMatch.Value, String.Empty);
                }
            }

            MatchCollection actionLinkMatches = RegularExpressions.ActionLinkServerTag.Matches(html);

            foreach (Match actionLinkMatch in actionLinkMatches)
            {
                string actionLink = LinkCreator.CreatorActionLink(this, context, actionLinkMatch);
                html = html.Replace(actionLinkMatch.Value, actionLink);
            }

            MatchCollection routeLinkMatches = RegularExpressions.RouteLinkServerTag.Matches(html);

            foreach (Match routeLinkMatch in routeLinkMatches)
            {
                string routeLink = LinkCreator.CreatorRouteLink(this, context, routeLinkMatch);
                html = html.Replace(routeLinkMatch.Value, routeLink);
            }

            html = RegularExpressions.LinkUrl.Replace(html, m =>
            {
                string group1 = m.Result("$1"), group2 = m.Result("$2"), group3 = m.Result("$3"), group5 = m.Result("$5");
                string applicationPath = context.HttpContext.Request.ApplicationPath != null ?
                                             context.HttpContext.Request.ApplicationPath.TrimEnd('/') :
                                             String.Empty;

                return String.Concat(group1, group2, group3, applicationPath, group5);
            });

            html = RegularExpressions.VersionVariable.Replace(html, m =>
            {
                string group1 = m.Result("$1"), group2 = m.Result("$2"), group3 = m.Result("$3"), group5 = m.Result("$5");

                return String.Concat(group1, group2, group3, !String.IsNullOrWhiteSpace(m_version) ? m_version : "0", group5);
            });

            html = RegularExpressions.ServerCommentDirective.Replace(html, String.Empty);

            return html;
        }

        private string ParsePartialViewContent(ViewContext context, Match match)
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
                    return GeneratePartialView(context, partialViewValue);
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

            string html = ReadViewHtml(partialViewFilePath, ViewType.PartialView);
            html = ParseViewContent(context, html);

            partialViewStack.Pop();

            return html;
        }

        private string PostRender(ViewContext viewContext, string html, object model)
        {
            if (m_antiForgeryTokenSupport)
            {
                Match antiForgeryMatch = RegularExpressions.AntiForgeryServerTag.Match(html);

                if (antiForgeryMatch.Success)
                {
                    var helper = new HtmlHelper(viewContext, this);

                    string tokenTag = helper.AntiForgeryToken().ToHtmlString().TrimLine();
                    html = RegularExpressions.AntiForgeryServerTag.Replace(html, tokenTag);
                }
            }

            if (m_modelPropertyName != null && RegularExpressions.ModelPropertyName.IsMatch(m_modelPropertyName))
            {
                Match modelMatch = RegularExpressions.ModelServerTag.Match(html);

                if (modelMatch.Success)
                {
                    html = RegularExpressions.ModelServerTag.Replace(html, ModelScriptTagCreator.Create(m_serializer, m_modelPropertyName, model));
                }
            }

            return html;
        }

        private string MapPath(ControllerContext context, string viewPath)
        {
            if (viewPath.IndexOf('~') == 0)
            {
                return context.HttpContext.Server.MapPath(viewPath.Trim());
            }

            return m_filePath.Substring(0, m_filePath.LastIndexOf('\\') + 1) + viewPath.Trim();
        }
    }
}
