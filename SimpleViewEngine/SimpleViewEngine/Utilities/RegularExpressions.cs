using System.Text.RegularExpressions;

namespace SimpleViewEngine.Utilities
{
    internal static class RegularExpressions
    {
        public readonly static Regex AntiForgeryServerTag = new Regex(@"<srv:anti\-forgery\s*/?>",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BaseServerTag = new Regex(@"<srv:base([\s/]+.*?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BaseTag = new Regex(@"<base[\s/]+.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BodyServerTag = new Regex(@"<srv:body\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex CssBundleServerTag = new Regex(@"<srv:css\-bundle\s+url\s*=\s*\""([^\s]*)\""\s*/?>",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex DebugServerTag = new Regex(@"<srv:debug>(.*?)</srv:debug>",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex DoNotMinifyTag = new Regex(@"<srv:minification\s*disabled(=\"".*?\"")?\s*/?>",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex LinkUrl = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
                                                         RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelServerTag = new Regex(@"<srv:model\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelPropertyName = new Regex(@"^[_$a-zA-Z][_$a-zA-Z0-9]+$", RegexOptions.Compiled);

        public readonly static Regex LayoutServerTag = new Regex(@"<srv:layout\s+(.+?)\s*=\s*\""(.+?)\""\s*/?>",
                                                                 RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex NamespaceAttribute = new Regex(@"<html\s+.*?(\s*xmlns:srv\s*=\s*[\""'].*?[\""'])",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex PartialServerTag = new Regex(@"<srv:partial\s+(.+?)\s*=\s*\""(.+?)\""\s*/?>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ReleaseServerTag = new Regex(@"<srv:release>(.*?)</srv:release>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex ScriptBundleServerTag = new Regex(@"<srv:script\-bundle\s+url\s*=\s*\""([^\s]*)\""\s*/?>",
                                                                       RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex SectionServerTag = new Regex(@"<srv:section\s+name\s*=\s*\""([A-Za-z_][A-Za-z0-9\-_]*)\""\s*/?>",
                                                        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex ServerCommentDirective = new Regex(@"<!--@(.*?)-->[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);

        public readonly static Regex TitleServerTag = new Regex(@"<srv:title>(.*?)</srv:title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex TitleTag = new Regex(@"<title>.*?</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex VersionVariable = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'].*)(\:version)([^\""^']*[\""'])",
                                                                 RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex WhiteSpaceBetweenTags = new Regex(@">(?! )\s+", RegexOptions.Compiled);

        public readonly static Regex WhiteSpaceBetweenLines = new Regex(@"([\n\s])+?(?<= {2,})<", RegexOptions.Compiled);

        public readonly static string CustomSectionTagTemplate = @"<srv:{0}>(.*?)</srv:{0}>";
    }
}
