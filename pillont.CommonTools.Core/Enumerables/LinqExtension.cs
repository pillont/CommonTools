using System;
using System.Collections.Generic;
using System.Linq;

namespace pillont.CommonTools.Core.Enumerables
{
    public static class LinqExtension
    {
        public static T[] AsArray<T>(this T value)
        {
            if (value == null)
            {
                return new T[0];
            }

            return new[] { value };
        }

        public static List<T> AsList<T>(this T value)
        {
            if (value == null)
            {
                return new List<T>();
            }

            return new List<T>() { value };
        }

        public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> list)
        {
            return list ?? new List<T>();
        }

        public static IEnumerable<TSource> Distinct<TSource, TFilter>(this IEnumerable<TSource> source, Func<TSource, TFilter> predicate)
        {
            return source.AsNotNull()
                         .GroupBy(predicate)
                         .Select(gr => gr.FirstOrDefault());
        }

        public static IEnumerable<T> IntersectBy<T>(this IEnumerable<IEnumerable<T>> allProductLists, Func<T, T, bool> areSame)
        {
            if (allProductLists.IsNullOrEmpty())
            {
                return new List<T>();
            }

            var firstList = allProductLists.FirstOrDefault();
            var othersLists = allProductLists.Skip(1);

            return firstList.Where(product =>
            product.ContainedInAll(othersLists, areSame))
                .ToList();
        }

        public static IEnumerable<TSource> JoinBy<TSource, TFilter>(this IEnumerable<TSource> source,
                    IEnumerable<TFilter> list,
            Func<TSource, TFilter> predicate)
        {
            return source.AsNotNull()
                          .Join(list.AsNotNull(),
                                predicate,
                                filter => filter,
                                (item, filter) => item);
        }
    }
}