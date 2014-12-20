using System.Text.RegularExpressions;

namespace SimpleViewEngine.Utilities
{
    internal static class RegularExpressions
    {
        public readonly static Regex AntiForgeryDirective = new Regex(@"<!--\s*#anti\-forgery\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex Base = new Regex(@"<base\s*.*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BaseDirective = new Regex(@"<!--\s*#base\s+href\s*=\s*\""([^\s]*)\""(\s+target\s*=\s*\""([^\s]*)\"")?\s*?-->",
                                                               RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex BodyDirective = new Regex(@"<!--\s*#body\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex CssBundle = new Regex(@"<css\-bundle\s+href\s*=\s*\""([^\s]*)\""\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex HeadBodyDirective = new Regex(@"<!--\s*#head(.*)/#head\s*-->",
                                                                   RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex HeadDirective = new Regex(@"<!--\s*#head\s*-->",
                                                               RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex Link = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelDirective = new Regex(@"<!--\s*#model\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ModelPropertyNameDirective = new Regex(@"^[_$a-zA-Z][_$a-zA-Z0-9]+$", RegexOptions.Compiled);

        public readonly static Regex PartialViewDirective = new Regex(@"<!--\s*#partial\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ReferenceDirective = new Regex(@"<!--\s*#layout\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ScriptBundle = new Regex(@"<script\-bundle\s+href\s*=\s*\""([^\s]*)\""\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex ScriptsBodyDirective = new Regex(@"<!--\s*#scripts(.*)/#scripts\s*-->",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex ScriptsDirective = new Regex(@"<!--\s*#scripts\s*-->",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public readonly static Regex ServerCommentDirective = new Regex(@"<!--@[^(-->)]*-->[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);

        public readonly static Regex TitleDirective = new Regex(@"<!--\s*#title\s+text=\""(.*)\""\s*-->",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex Title = new Regex(@"<title>.*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly static Regex Version = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'].*)(\:version)([^\""^']*[\""'])",
                                                         RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
