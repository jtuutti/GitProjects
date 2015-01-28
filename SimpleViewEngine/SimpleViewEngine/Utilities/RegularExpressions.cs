using System.Text.RegularExpressions;

namespace SimpleViewEngine.Utilities
{
    internal static class RegularExpressions
    {
        public readonly static Regex ActionLinkServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":action-link(\s+.*?)\s*>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":action-link>",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex AntiForgeryServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":anti\-forgery\s*/?>",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex AttributeNameValue = new Regex(@"\s+(.+?)\s*=\s*\""([^\""]*)\""",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BaseServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":base([\s/]+.*?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BaseTag = new Regex(@"<base[\s/]+.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BodyServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":body\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex CssBundleServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":css\-bundle\s+url\s*=\s*\""([^\s]*)\""\s*/?>",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex DebugServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":debug>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":debug>",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex DoNotMinifyTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":minification\s*disabled(=\""[^\""]*\"")?\s*/?>",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex LinkUrl = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
                                                         RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":model\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelPropertyName = new Regex(@"^[_$a-zA-Z][_$a-zA-Z0-9]+$", RegexOptions.Compiled);

        public readonly static Regex LayoutServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":layout\s+(.+?)\s*=\s*\""([^\""]+)\""\s*/?>",
                                                                 RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex NamespaceAttribute = new Regex(@"<html\s+.*?(\s*xmlns:" + HtmlViewEngine.ServerTagPrefix + @"\s*=\s*[\""'][^\""]*[\""'])",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex PartialServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":partial\s+(.+?)\s*=\s*\""([^\""]+)\""\s*/?>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ReleaseServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":release>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":release>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex RouteLinkServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":route-link(\s+.*?)\s*>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":route-link>",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ScriptBundleServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":script\-bundle\s+url\s*=\s*\""([^\s]*)\""\s*/?>",
                                                                       RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex SectionServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":section\s+name\s*=\s*\""([A-Za-z_][A-Za-z0-9\-_]*)\""\s*/?>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex ServerCommentDirective = new Regex(@"<!--@(.*?)-->[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);

        public readonly static Regex TitleServerTag = new Regex(@"<" + HtmlViewEngine.ServerTagPrefix + @":title>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":title>",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex TitleTag = new Regex(@"<title>.*?</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex VersionVariable = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'].*)(\:version)([^\""^']*[\""'])",
                                                                 RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex WhiteSpaceBetweenTags = new Regex(@">(?! )\s+", RegexOptions.Compiled);

        public readonly static Regex WhiteSpaceBetweenLines = new Regex(@"([\n\s])+?(?<= {2,})<", RegexOptions.Compiled);

        public readonly static string CustomSectionTagTemplate = @"<" + HtmlViewEngine.ServerTagPrefix + @":{0}>(.*?)</" + HtmlViewEngine.ServerTagPrefix + @":{0}>";
    }
}
