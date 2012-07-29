// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace RestFoundation.Odata.Parser
{
    internal static class ExpressionTokenizer
    {
        public static IEnumerable<TokenSet> GetTokens(this string expression)
        {
            var cleanMatch = expression.EnclosedMatch();

            if (cleanMatch.Success)
            {
                var match = cleanMatch.Groups[1].Value;
                if (!HasOrphanedOpenParenthesis(match))
                {
                    expression = match;
                }
            }

            if (expression.IsImpliedBoolean())
            {
                yield break;
            }

            var blocks = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var openGroups = 0;
            var startExpression = 0;
            var currentTokens = new TokenSet();

            for (int i = 0; i < blocks.Length; i++)
            {
                var netEnclosed = blocks[i].Count(c => c == '(') - blocks[i].Count(c => c == ')');
                openGroups += netEnclosed;

                if (openGroups == 0)
                {
                    if (blocks[i].IsOperation())
                    {
                        var expression1 = startExpression;
                        var i1 = i;
                        Func<string, int, bool> predicate = (x, j) => j >= expression1 && j < i1;

                        if (string.IsNullOrWhiteSpace(currentTokens.Left))
                        {
                            currentTokens.Left = string.Join(" ", blocks.Where(predicate));
                            currentTokens.Operation = blocks[i];
                            startExpression = i + 1;

                            if (blocks[i].IsCombinationOperation())
                            {
                                currentTokens.Right = string.Join(" ", blocks.Where((x, j) => j > i));

                                yield return currentTokens;
                                yield break;
                            }
                        }
                        else
                        {
                            currentTokens.Right = string.Join(" ", blocks.Where(predicate));

                            yield return currentTokens;

                            startExpression = i + 1;
                            currentTokens = new TokenSet();

                            if (blocks[i].IsCombinationOperation())
                            {
                                yield return new TokenSet { Operation = blocks[i].ToLowerInvariant() };
                            }
                        }
                    }
                }
            }

            var remainingToken = string.Join(" ", blocks.Where((x, j) => j >= startExpression));

            if (!string.IsNullOrWhiteSpace(currentTokens.Left))
            {
                currentTokens.Right = remainingToken;
                yield return currentTokens;
            }
            else if (remainingToken.IsEnclosed())
            {
                currentTokens.Left = remainingToken;
                yield return currentTokens;
            }
        }

        public static TokenSet GetArithmeticToken(this string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var cleanMatch = expression.EnclosedMatch();

            if (cleanMatch.Success)
            {
                var match = cleanMatch.Groups[1].Value;
                if (!HasOrphanedOpenParenthesis(match))
                {
                    expression = match;
                }
            }

            var blocks = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var hasOperation = blocks.Any(x => x.IsArithmetic());
            if (!hasOperation)
            {
                return null;
            }

            var operationIndex = GetArithmeticOperationIndex(blocks);

            var left = string.Join(" ", blocks.Where((x, i) => i < operationIndex));
            var right = string.Join(" ", blocks.Where((x, i) => i > operationIndex));
            var operation = blocks[operationIndex];

            return new TokenSet { Left = left, Operation = operation, Right = right };
        }

        private static int GetArithmeticOperationIndex(IList<string> blocks)
        {
            if (blocks == null)
            {
                throw new ArgumentNullException("blocks");
            }

            var openGroups = 0;
            var operationIndex = -1;
            for (var i = 0; i < blocks.Count; i++)
            {
                var source = blocks[i];

                var netEnclosed = source.Count(c => c == '(') - source.Count(c => c == ')');
                openGroups += netEnclosed;

                if (openGroups == 0 && source.IsArithmetic())
                {
                    operationIndex = i;
                }
            }

            return operationIndex;
        }

        private static bool HasOrphanedOpenParenthesis(string expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var opens = new List<int>();
            var closes = new List<int>();
            var index = expression.IndexOf('(');
            while (index > -1)
            {
                opens.Add(index);
                index = expression.IndexOf('(', index + 1);
            }

            index = expression.IndexOf(')');
            while (index > -1)
            {
                closes.Add(index);
                index = expression.IndexOf(')', index + 1);
            }

            var pairs = opens.Zip(closes, (o, c) => new Tuple<int, int>(o, c));
            var hasOrphan = opens.Count == closes.Count && pairs.Any(x => x.Item2 < x.Item1);

            return hasOrphan;
        }
    }
}
