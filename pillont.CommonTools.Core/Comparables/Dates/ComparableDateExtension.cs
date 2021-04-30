using System;
using System.Linq.Expressions;

namespace pillont.CommonTools.Core.Comparables.Dates
{
    public static class ComparableDateExtension
    {
        public static bool CompareTo(this ComparableDate source, DateTime? date)
        {
            ThrowIfArgumentIsNull(source);

            Func<DateTime?, bool> lambda = GetComparisonExpression(source)
                .Compile();

            bool result = lambda(date);
            return result;
        }

        ///<summary>
        ///INPUT :
        /// nullableDateExp : product => product.Date
        /// filter.ComparatorType : Equal
        /// EXPRESSION GÉNÉRÉE :
        ///
        /// product => product.Date != null                                  // on check que les dates non nulles
        /// && (
        ///     (
        ///          source.DateType == DateType.Date                        // si le type est Day : on applique .Day sur les dates
        ///          && (Date)(object)product.Date.Value.Date                // (Date)(object) pour caster les champs en SmallDate
        ///                                         == source.Date.Date      // on compare avec la valeur du filtre
        ///     ) || (
        ///          source.DateType != DateType.Day                        // si le type n'est pas Day : on prend directement les dates
        ///          && (Date)(object)product.EndDate.Value                 // (Date)(object) pour caster les champs en SmallDate
        ///                                         == source.Date          // on compare avec la valeur du filtre
        ///     )
        /// )
        ///
        /// </summary>
        /// <example>
        /// IQueryable<BillingProductEntity> filterQueryByEndDate(IQueryable<BillingProductEntity> query, ComparableDate source)
        /// {
        ///      // (BillingProductEntity product) => product.EndDate
        ///      var inputExp = Expression.Parameter(typeof(BillingProductEntity));
        ///      Expression getDateExp = Expression.Property(inputExp, nameof(BillingProductEntity.EndDate));
        ///
        ///      // filtre
        ///      Expression checkDateExp = IsValidExp(getDateExp, source);
        ///      var lambda = Expression.Lambda<Func<BillingProductEntity, bool>>(checkDateExp, new[] { inputExp });
        ///      return query.Where(lambda);
        /// }
        /// </example>
        /// <param name="nullableDateExp">expression pour récupérer une date</param>
        public static BinaryExpression GetComparisonExpression(this ComparableDate filter, Expression nullableDateExp)
        {
            ThrowIfArgumentIsNull(filter);

            // (Date)(object)product.Date.Value
            Expression getDateExp = GetValidDate(nullableDateExp);
            getDateExp = ApplyDateType(filter.DateType, getDateExp);

            // source.Date
            Expression getFilterExp = Expression.Constant(filter.Date);
            getFilterExp = ApplyDateType(filter.DateType, getFilterExp);

            // date == filter
            Expression applyOperatorExp = ComparisonHelper.ApplyCompareOperation(filter.ComparatorType, getDateExp, getFilterExp);

            // product.Date != null &&
            BinaryExpression checkExp = ApplyNullCheck(nullableDateExp, applyOperatorExp);
            return checkExp;
        }

        public static Expression<Func<DateTime?, bool>> GetComparisonExpression(this ComparableDate source)
        {
            var inputExp = Expression.Parameter(typeof(DateTime?));
            var checkDateExp = GetComparisonExpression(source, inputExp);

            Expression<Func<DateTime?, bool>> lambda = Expression.Lambda<Func<DateTime?, bool>>(checkDateExp, new[] { inputExp });
            return lambda;
        }

        public static Expression<Func<T, bool>> GetDateComparisonExpression<T>(this ComparableDate source, string propertyName)
        {
            var inputExp = Expression.Parameter(typeof(T));
            var getDateExp = Expression.Property(inputExp, propertyName);
            var checkDateExp = source.GetComparisonExpression(getDateExp);
            return Expression.Lambda<Func<T, bool>>(checkDateExp, new[] { inputExp });
        }

        private static Expression ApplyDateType(DateType dateType, Expression dateExpression)
        {
            if (dateType == DateType.Date)
            {
                return Expression.Property(dateExpression, nameof(DateTime.Date));
            }

            return dateExpression;
        }

        private static BinaryExpression ApplyNullCheck(Expression nullableDateExp, Expression applyOperatorExp)
        {
            var hasValueExpression = Expression.Property(nullableDateExp, nameof(Nullable<DateTime>.HasValue));
            var checkExp = Expression.AndAlso(hasValueExpression, applyOperatorExp);
            return checkExp;
        }

        private static Expression GetValidDate(Expression nullableDateExp)
        {
            Expression getNotNullValueExp = Expression.Property(nullableDateExp, nameof(Nullable<DateTime>.Value));

            // NOTE : si la date est en smalldatetime : la comparaison avec une date entière ne marche pas
            // HACK : on cast en object puis en date pour forcé le type en date entière
            // EXCEPTION : SqlException : Échec de la conversion d'une chaîne de caractères en type de données smalldatetime
            Expression getAsObjectExp = Expression.Convert(getNotNullValueExp, typeof(object));
            Expression getDateExp = Expression.Convert(getAsObjectExp, typeof(DateTime));
            return getDateExp;
        }

        private static void ThrowIfArgumentIsNull(ComparableDate source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
        }
    }
}
