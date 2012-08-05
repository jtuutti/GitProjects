// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Text.RegularExpressions;

namespace RestFoundation
{
    /// <summary>
    /// Represents a regular expression constraint for a service method parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ParameterConstraintAttribute : Attribute
    {
        private const char StartPatternSymbol = '^';
        private const char EndPatternSymbol = '$';

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterConstraintAttribute"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        public ParameterConstraintAttribute(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            PatternRegex = new Regex(String.Concat(StartPatternSymbol, pattern.TrimStart(StartPatternSymbol).TrimEnd(EndPatternSymbol), EndPatternSymbol),
                                     RegexOptions.CultureInvariant);
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the regular expression pattern of the constraint.
        /// </summary>
        public string Pattern { get; private set; }

        internal Regex PatternRegex { get; private set; }
    }
}
