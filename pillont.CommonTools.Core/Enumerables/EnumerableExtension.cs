using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hellowork.BackOffice.Tools.Enumerables
{
    public static class EnumerableHelper
    {
        private const int MinChunkSize = 1;

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        /// SOURCE : https://stackoverflow.com/a/6362642
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize < MinChunkSize)
            {
                throw new InvalidOperationException($"{nameof(chunksize)} must be bigger than {MinChunkSize}");
            }

            source = source ?? new List<T>();

            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static bool ContainedInAll<T>(this T product, IEnumerable<IEnumerable<T>> allLists, Func<T, T, bool> areSame)
        {
            if (allLists.IsNullOrEmpty())
            {
                return false;
            }

            return allLists
                .AsNotNull()
                .All(list => list
                    .AsNotNull()
                    .Any(pr => areSame(pr, product))
            );
        }

        public static bool HasValues(this IEnumerable collection)
        {
            return !collection.IsNullOrEmpty();
        }

        [Obsolete("Exception for a query ! Better to apply `ToList()`, store the result and apply this func on it, to apply only once execution of the query", true)]
        public static bool IsNullOrEmpty(this IQueryable collection)
        {
            return IsNullOrEmpty(collection as IEnumerable);
        }

        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            if (collection is IQueryable)
            {
                throw new InvalidOperationException("the list is a query, IsNullOrEmpty will execute query only for empty check... \n" +
                    "Better to apply `ToList()`, store the result and apply this func on it, to apply only once execution of the query");
            }

            return collection == null
                            || !collection.GetEnumerator().MoveNext();
        }
    }
}