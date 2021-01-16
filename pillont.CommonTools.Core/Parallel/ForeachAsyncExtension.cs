using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pillont.CommonTools.Core.Enumerables;

namespace pillont.CommonTools.Core.Parallel
{
    public static class ForeachAsyncExtension
    {
        /// <summary>
        /// lance plusieurs collectes asynchrone en parallèle et agglomère les resultats en une seule liste
        /// </summary>
        public static async Task<IReadOnlyCollection<TResult>> AggregateForeachAsync<TElement, TResult>(this IEnumerable<TElement> collection, Func<TElement, Task<IEnumerable<TResult>>> action)
        {
            var allLists = await ForeachAsync(collection, action);
            return allLists.AsParallel()
                            .SelectMany(list => list.AsNotNull())
                            .ToList();
        }

        /// <summary>
        /// fait un foreach asynchrone sur une liste
        /// </summary>
        /// <typeparam name="TElement">type dans la liste</typeparam>
        /// <param name="collection">source</param>
        /// <param name="action">action async à appliquer sur chaque element de la liste</param>
        /// <returns></returns>
        public static async Task ForeachAsync<TElement>(this IEnumerable<TElement> collection, Func<TElement, Task> action)
        {
            var errors = new ConcurrentBag<Exception>();
            await UnsafeForeachAsync(collection, async e =>
            {
                try
                {
                    await action(e);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            });

            if (errors.Any())
            {
                throw new AggregateException("errors during foreach async", errors);
            }
        }

        /// <summary>
        /// fait un foreach asynchrone sur une liste
        /// récupère tous les résultats et les retournes dans une liste
        /// </summary>
        /// <typeparam name="TElement">type dans la liste source</typeparam>
        /// <typeparam name="TResult">type dans la liste résultante</typeparam>
        /// <param name="collection">source</param>
        /// <param name="action">action async à appliquer sur chaque element de la liste</param>
        /// <returns>liste de tous les résultats des actions exécutées sur chaque éléments de la liste source</returns>
        public static async Task<IReadOnlyCollection<TResult>> ForeachAsync<TElement, TResult>(this IEnumerable<TElement> collection, Func<TElement, Task<TResult>> action)
        {
            var result = new ConcurrentBag<TResult>();

            await ForeachAsync(collection, async e =>
            {
                var r = await action(e);
                result.Add(r);
            });

            return result;
        }

        private static async Task UnsafeForeachAsync<TElement>(this IEnumerable<TElement> collection, Func<TElement, Task> action)
        {
            var allTasks = collection.Select(e => action(e));
            await Task.WhenAll(allTasks);
        }
    }
}