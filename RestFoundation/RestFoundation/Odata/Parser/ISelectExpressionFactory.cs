// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Linq.Expressions;

namespace RestFoundation.Odata.Parser
{
    internal interface ISelectExpressionFactory<T>
    {
        Expression<Func<T, object>> Create(string selection);
    }
}