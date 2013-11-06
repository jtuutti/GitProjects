using System.Text.RegularExpressions;

namespace SimpleViewEngine.Utilities
{
    internal static class RegularExpressions
    {
        public static readonly Regex BodyDirective = new Regex(@"<!--\s*#body\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex HeadBodyDirective = new Regex(@"<!--\s*#head(.*)/#head\s*-->",
                                                                   RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex HeadDirective  = new Regex(@"<!--\s*#head\s*-->",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        public static readonly Regex Link = new Regex(@"(\s+)(src|href|data\-[A-Za-z0-9_\-]*\-?href)(\s*=\s*[\""'])(~)([^\""^']*[\""'])",
                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex ModelDirective = new Regex(@"<!--\s*#model\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex PartialViewDirective = new Regex(@"<!--\s*#partial\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex ReferenceDirective = new Regex(@"<!--\s*#layout\s+(.+)\s*=\s*\""(.+)\""\s*-->",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex ScriptsBodyDirective = new Regex(@"<!--\s*#scripts(.*)/#scripts\s*-->",
                                                                      RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex ScriptsDirective = new Regex(@"<!--\s*#scripts\s*-->",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex TitleDirective = new Regex(@"<!--\s*#title text=\""(.*)\""\s*-->",
                                                                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex Title = new Regex(@"<title>.*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
