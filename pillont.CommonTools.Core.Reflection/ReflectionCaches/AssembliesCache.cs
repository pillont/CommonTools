using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using pillont.CommonTools.Core.Parallel;

namespace pillont.CommonTools.Core.Reflection.ReflectionCaches
{
    internal sealed class AssembliesCache
    {
        /// <summary>
        /// cache to update collect performance
        /// </summary>
        private IDictionary<string, Assembly> m_CacheByFilter;

        /// <summary>
        /// semaphore locker to make <see cref="m_CacheByFilter"/> thread safe
        /// </summary>
        /// <remarks>
        /// NOTE: with async func, use semaphore, never use "lock" statement :
        /// SOURCE : https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
        /// </remarks>
        private SemaphoreSlim m_Locker = new SemaphoreSlim(1, 1);

        public IList<Assembly> CollectWithCache()
        {
            TryPopulateCache();
            return m_CacheByFilter?.Values?.ToList();
        }

        public Assembly CollectWithCache(String p_Assemply)
        {
            TryPopulateCache();
            if (m_CacheByFilter.ContainsKey(p_Assemply))
                return m_CacheByFilter[p_Assemply];
            else
                return null;
        }

        private static IDictionary<string, Assembly> CollectToPopulateCache()
        {
            IDictionary<string, Assembly> v_Dic = new ConcurrentDictionary<string, Assembly>();
            //distinct by fullname
            Dictionary<string, Assembly> v_Temp = AppDomain.CurrentDomain.GetAssemblies()
                .Where(c => !c.IsDynamic)
                .GroupBy(x => x.FullName)
                .ToDictionary(v_Assembly => v_Assembly.Key, v_Assembly => v_Assembly.First());

            //dictionary with name and assembly
            foreach (KeyValuePair<string, Assembly> v_Assembly in v_Temp)
                v_Dic.Add(v_Assembly.Value.GetName().Name, v_Assembly.Value);

            return v_Dic;
        }

        private void TryPopulateCache()
        {
            //quick test
            if (m_CacheByFilter?.Count > 0)
                return;

            m_Locker.WaitFor(millisecondsTimeout: 1500, action: () =>
            {
                if (m_CacheByFilter?.Count > 0)
                    return;

                m_CacheByFilter = CollectToPopulateCache();
            });
        }
    }
}