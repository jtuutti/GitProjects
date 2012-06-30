// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace RestFoundation.Odata.Parser
{
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
            return string.Format("{0} {1} {2}", Left, Operation, Right);
        }
    }
}
