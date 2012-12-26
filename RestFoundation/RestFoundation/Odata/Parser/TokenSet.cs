// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RestFoundation.Odata.Parser
{
    [ExcludeFromCodeCoverage]
    internal class TokenSet
    {
        public TokenSet()
        {
            Left = string.Empty;
            Right = string.Empty;
            Operation = string.Empty;
        }

        public string Left { get; set; }
        public string Operation { get; set; }
        public string Right { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Left, Operation, Right);
        }
    }
}
