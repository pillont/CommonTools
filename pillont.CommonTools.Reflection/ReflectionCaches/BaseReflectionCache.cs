using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using pillont.CommonTools.Core.Parallel;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    /// <summary>
    /// abstract class to make cache
    /// </summary>
    /// <typeparam name="TFilter">type of the object to collect value</typeparam>
    /// <typeparam name="TResult">type of result returned</typeparam>
    public abstract class BaseReflectionCache<TFilter, TResult>
    {
        /// <summary>
        /// cache to update collect performance
        /// </summary>
        private readonly IDictionary<TFilter, TResult> m_CacheByFilter;

        /// <summary>
        /// Number of maximum items to store in cache
        /// </summary>
        private readonly int m_LimitCache = -1;

        /// <summary>
        /// semaphore locker to make <see cref="m_CacheByFilter"/> thread safe
        /// </summary>
        /// <remarks>
        /// NOTE: with async func, use semaphore, never use "lock" statement :
        /// SOURCE : https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
        /// </remarks>
        private SemaphoreSlim m_DictionaryLocker = new SemaphoreSlim(1, 1);

        /// <summary>
        /// ctor
        /// </summary>
        public BaseReflectionCache()
            : this(new ConcurrentDictionary<TFilter, TResult>())
        { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="p_LimitCache">Number of maximum items to store in cache</param>
        public BaseReflectionCache(int p_LimitCache)
            : this(new ConcurrentDictionary<TFilter, TResult>(), p_LimitCache)
        { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="p_CacheByFilter">dictionnary to stock values in cache</param>
        /// <param name="p_LimitCache">Number of maximum items to store in cache</param>
        protected BaseReflectionCache(IDictionary<TFilter, TResult> p_CacheByFilter, int p_LimitCache = -1)
        {
            if (p_CacheByFilter == null)
                throw new ArgumentNullException(nameof(p_CacheByFilter), "Dictionary cannot be null");

            m_LimitCache = p_LimitCache;
            m_CacheByFilter = p_CacheByFilter ?? throw new ArgumentNullException(nameof(p_CacheByFilter));
        }

        /// <summary>
        /// collect value with cache
        /// the first time, the internal collect will be done and put in cache
        /// the others times, the result will be cache result
        /// </summary>
        /// <exception cref="ArgumentNullException">type is null</exception>
        public TResult CollectWithCache(TFilter p_Filter)
        {
            if (p_Filter == null)
                throw new ArgumentNullException(nameof(p_Filter));

            TryPopulateCache(p_Filter);
            return m_CacheByFilter[p_Filter];
        }

        /// <summary>
        /// intern collect after populate cache
        /// </summary>
        protected abstract TResult CollectToPopulateCache(TFilter p_Filter);

        /// <summary>
        /// check if cache contains filter
        /// if not contains => collect and add value
        /// thread safe populate cache
        /// </summary>
        /// <seealso cref="UnsafeTryPopulateCache(TFilter)"/>
        private void TryPopulateCache(TFilter p_Filter)
        {
            //quick test exist in cache
            if (m_LimitCache < 1 && m_CacheByFilter.ContainsKey(p_Filter))
                return;

            m_DictionaryLocker.WaitFor(millisecondsTimeout: 1500, action: () =>
            {
                //clean cache if number of limit cache is reach
                if (m_LimitCache > 0 && m_CacheByFilter.Count == m_LimitCache)
                    m_CacheByFilter.Clear();

                //exist in cache
                if (m_CacheByFilter.ContainsKey(p_Filter))
                    return;

                m_CacheByFilter[p_Filter] = CollectToPopulateCache(p_Filter);
            });
        }
    }
}