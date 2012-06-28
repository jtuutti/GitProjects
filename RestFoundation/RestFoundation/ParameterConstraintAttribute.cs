﻿using System;
using System.Text.RegularExpressions;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ParameterConstraintAttribute : Attribute
    {
        private const char StartPatternSymbol = '^';
        private const char EndPatternSymbol = '$';

        public ParameterConstraintAttribute(string pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            Pattern = new Regex(String.Concat(StartPatternSymbol, pattern.TrimStart(StartPatternSymbol).TrimEnd(EndPatternSymbol), EndPatternSymbol),
                                RegexOptions.CultureInvariant);
        }

        internal Regex Pattern { get; private set; }
    }
}