// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RestFoundation.Odata
{
    internal static class MethodProvider
    {
        public static readonly ConstantExpression IgnoreCaseExpression;

        private static readonly MethodInfo innerContainsMethod;
        private static readonly MethodInfo innerIndexOfMethod;
        private static readonly MethodInfo endsWithMethod1;
        private static readonly MethodInfo startsWithMethod1;
        private static readonly PropertyInfo lengthProperty1;
        private static readonly MethodInfo substringMethod1;
        private static readonly MethodInfo toLowerMethod1;
        private static readonly MethodInfo toUpperMethod1;
        private static readonly MethodInfo trimMethod1;
        private static readonly PropertyInfo dayProperty1;
        private static readonly PropertyInfo hourProperty1;
        private static readonly PropertyInfo minuteProperty1;
        private static readonly PropertyInfo secondProperty1;
        private static readonly PropertyInfo monthProperty1;
        private static readonly PropertyInfo yearProperty1;
        private static readonly MethodInfo doubleRoundMethod1;
        private static readonly MethodInfo decimalRoundMethod1;
        private static readonly MethodInfo doubleFloorMethod1;
        private static readonly MethodInfo decimalFloorMethod1;
        private static readonly MethodInfo doubleCeilingMethod1;
        private static readonly MethodInfo decimalCeilingMethod1;

        static MethodProvider()
        {
            var stringType = typeof(string);
            var datetimeType = typeof(DateTime);
            var mathType = typeof(Math);
            var stringComparisonType = typeof(StringComparison);

            IgnoreCaseExpression = Expression.Constant(StringComparison.OrdinalIgnoreCase);

            innerContainsMethod = stringType.GetMethod("Contains", new[] { stringType });
            innerIndexOfMethod = stringType.GetMethod("IndexOf", new[] { stringType, stringComparisonType });
            endsWithMethod1 = stringType.GetMethod("EndsWith", new[] { stringType, stringComparisonType });
            startsWithMethod1 = stringType.GetMethod("StartsWith", new[] { stringType, stringComparisonType });
            lengthProperty1 = stringType.GetProperty("Length", Type.EmptyTypes);
            substringMethod1 = stringType.GetMethod("Substring", new[] { typeof(int) });
            toLowerMethod1 = stringType.GetMethod("ToLowerInvariant", Type.EmptyTypes);
            toUpperMethod1 = stringType.GetMethod("ToUpperInvariant", Type.EmptyTypes);
            trimMethod1 = stringType.GetMethod("Trim", Type.EmptyTypes);

            dayProperty1 = datetimeType.GetProperty("Day", Type.EmptyTypes);
            hourProperty1 = datetimeType.GetProperty("Hour", Type.EmptyTypes);
            minuteProperty1 = datetimeType.GetProperty("Minute", Type.EmptyTypes);
            secondProperty1 = datetimeType.GetProperty("Second", Type.EmptyTypes);
            monthProperty1 = datetimeType.GetProperty("Month", Type.EmptyTypes);
            yearProperty1 = datetimeType.GetProperty("Year", Type.EmptyTypes);

            doubleRoundMethod1 = mathType.GetMethod("Round", new[] { typeof(double) });
            decimalRoundMethod1 = mathType.GetMethod("Round", new[] { typeof(decimal) });
            doubleFloorMethod1 = mathType.GetMethod("Floor", new[] { typeof(double) });
            decimalFloorMethod1 = mathType.GetMethod("Floor", new[] { typeof(decimal) });
            doubleCeilingMethod1 = mathType.GetMethod("Ceiling", new[] { typeof(double) });
            decimalCeilingMethod1 = mathType.GetMethod("Ceiling", new[] { typeof(decimal) });
        }

        public static MethodInfo IndexOfMethod
        {
            get
            {
                return innerIndexOfMethod;
            }
        }

        public static MethodInfo ContainsMethod
        {
            get
            {
                return innerContainsMethod;
            }
        }

        public static MethodInfo EndsWithMethod
        {
            get
            {
                return endsWithMethod1;
            }
        }

        public static MethodInfo StartsWithMethod
        {
            get
            {
                return startsWithMethod1;
            }
        }

        public static PropertyInfo LengthProperty
        {
            get
            {
                return lengthProperty1;
            }
        }

        public static MethodInfo SubstringMethod
        {
            get
            {
                return substringMethod1;
            }
        }

        public static MethodInfo ToLowerMethod
        {
            get
            {
                return toLowerMethod1;
            }
        }

        public static MethodInfo ToUpperMethod
        {
            get
            {
                return toUpperMethod1;
            }
        }

        public static MethodInfo TrimMethod
        {
            get
            {
                return trimMethod1;
            }
        }

        public static PropertyInfo DayProperty
        {
            get
            {
                return dayProperty1;
            }
        }

        public static PropertyInfo HourProperty
        {
            get
            {
                return hourProperty1;
            }
        }

        public static PropertyInfo MinuteProperty
        {
            get
            {
                return minuteProperty1;
            }
        }

        public static PropertyInfo SecondProperty
        {
            get
            {
                return secondProperty1;
            }
        }

        public static PropertyInfo MonthProperty
        {
            get
            {
                return monthProperty1;
            }
        }

        public static PropertyInfo YearProperty
        {
            get
            {
                return yearProperty1;
            }
        }

        public static MethodInfo DoubleRoundMethod
        {
            get
            {
                return doubleRoundMethod1;
            }
        }

        public static MethodInfo DecimalRoundMethod
        {
            get
            {
                return decimalRoundMethod1;
            }
        }

        public static MethodInfo DoubleFloorMethod
        {
            get
            {
                return doubleFloorMethod1;
            }
        }

        public static MethodInfo DecimalFloorMethod
        {
            get
            {
                return decimalFloorMethod1;
            }
        }

        public static MethodInfo DoubleCeilingMethod
        {
            get
            {
                return doubleCeilingMethod1;
            }
        }

        public static MethodInfo DecimalCeilingMethod
        {
            get
            {
                return decimalCeilingMethod1;
            }
        }
    }
}