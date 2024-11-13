using pillont.CommonTools.Core.Enumerables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pillont.CommonTools.Core.Parallel;

public static class ForeachAsyncExtension
{
    /// <summary>
    /// lance plusieurs collectes asynchrone en parallèle et agglomère les resultats en une seule liste
    /// </summary>
    public static async Task<IReadOnlyCollection<TResult>> AggregateForeachAsync<TElement, TResult>(
        this IEnumerable<TElement> collection,
        Func<TElement, CancellationToken, Task<IEnumerable<TResult>>> action,
        CancellationToken? cancellationToken = null)
    {
        var allLists = await ForeachAsync(collection, action, cancellationToken);
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
    public static async Task ForeachAsync<TElement>(
        this IEnumerable<TElement> collection,
        Func<TElement, CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)

    {
        var token = cancellationToken ?? CancellationToken.None;

        await collection.ForeachAsync((e, i, t) => action(e, t), token);
    }
    /// <summary>
    /// fait un foreach asynchrone sur une liste
    /// </summary>
    /// <typeparam name="TElement">type dans la liste</typeparam>
    /// <param name="collection">source</param>
    /// <param name="action">action async à appliquer sur chaque element de la liste</param>
    /// <returns></returns>
    public static async Task ForeachAsync<TElement>(
        this IEnumerable<TElement> collection,
        Func<TElement, int, CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var errors = new ConcurrentBag<Exception>();
        await UnsafeForeachAsync(collection, async (e, i, ct) =>
        {
            try
            {
                await action(e, i, ct);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        },
        cancellationToken);

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
    public static async Task<IReadOnlyCollection<TResult>> ForeachAsync<TElement, TResult>(
        this IEnumerable<TElement> collection,
        Func<TElement, CancellationToken, Task<TResult>> action,
        CancellationToken? cancellationToken = null)
    {
        var result = new ConcurrentBag<KeyValuePair<int, TResult>>();

        await ForeachAsync(collection, async (e, i, ct) =>
        {
            var r = await action(e, ct);
            result.Add(new KeyValuePair<int, TResult>(i, r));
        }, cancellationToken);

        return result
            .OrderBy(r => r.Key)
            .Select(r => r.Value)
            .ToList();
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
    public static async Task<IReadOnlyCollection<TResult>> ForeachAsync<TElement, TResult>(
        this IEnumerable<TElement> collection,
        Func<TElement, CancellationToken, TResult> action,
        CancellationToken? cancellationToken = null)
    {
        return await collection.ForeachAsync((t, ct) => Task.FromResult(action(t, ct)), cancellationToken);
    }

    private static async Task UnsafeForeachAsync<TElement>(
        this IEnumerable<TElement> collection,
        Func<TElement, int, CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;
        var allTasks = collection.Select((e, i) => action(e, i, token));

        await Task.WhenAll(allTasks).WaitAsync(token);
    }
}