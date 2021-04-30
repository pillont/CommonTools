using System.Linq.Expressions;

namespace pillont.CommonTools.Core.Comparables
{
    public static class ComparisonHelper
    {
        internal static BinaryExpression ApplyCompareOperation(
                    ComparatorType comparatorType,
            Expression getValueExp,
            Expression getFilterExp)
        {
            switch (comparatorType)
            {
                case ComparatorType.Equal:
                    return Expression.Equal(getValueExp, getFilterExp);

                case ComparatorType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(getValueExp, getFilterExp);

                case ComparatorType.GreaterThan:
                    return Expression.GreaterThan(getValueExp, getFilterExp);

                case ComparatorType.LessThan:
                    return Expression.LessThan(getValueExp, getFilterExp);

                case ComparatorType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(getValueExp, getFilterExp);

                default:
                    goto case ComparatorType.Equal;
            }
        }
    }
}
