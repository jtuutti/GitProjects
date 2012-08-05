// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestFoundation.Odata.Parser
{
    internal static class TokenOperatorExtensions
    {
        private static readonly string[] operations = new[] { "eq", "ne", "gt", "ge", "lt", "le", "and", "or", "not" };
        private static readonly string[] combiners = new[] { "and", "or", "not" };
        private static readonly string[] arithmetic = new[] { "add", "sub", "mul", "div", "mod" };

        private static readonly string[] booleanFunctions = new[] { "substringof", "endswith", "startswith" };
        private static readonly Regex cleanRx = new Regex(@"^\((.+)\)$", RegexOptions.Compiled);

        public static bool IsCombinationOperation(this string operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return combiners.Any(x => string.Equals(x, operation, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsOperation(this string operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return operations.Any(x => string.Equals(x, operation, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsArithmetic(this string operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return arithmetic.Any(x => string.Equals(x, operation, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsImpliedBoolean(this string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!string.IsNullOrWhiteSpace(expression) && !expression.IsEnclosed() && expression.IsFunction())
            {
                var split = expression.Split(' ');
                return !split.Intersect(operations).Any()
                        && !split.Intersect(combiners).Any()
                        && booleanFunctions.Any(x => split[0].StartsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public static Match EnclosedMatch(this string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return cleanRx.Match(expression);
        }

        public static bool IsEnclosed(this string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var match = expression.EnclosedMatch();
            return match != null && match.Success;
        }

        private static bool IsFunction(this string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var open = expression.IndexOf('(');
            var close = expression.IndexOf(')');

            return open > -1 && close > -1;
        }
    }
}