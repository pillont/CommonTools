using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using pillont.CommonTools.Core.Parallel;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    /// <summary>
    /// collect all types in assembly
    /// </summary>
    internal class TypesInAssemblyCache : ListReflectionCache<Assembly, Type>
    {
        /// <summary>
        /// dictionnary to make cache between type and assembly
        /// </summary>
        private readonly IDictionary<Type, Assembly> m_TypeToAssembly = new ConcurrentDictionary<Type, Assembly>();

        /// <summary>
        /// semaphore locker to make <see cref="m_TypeToAssembly"/> thread safe
        /// </summary>
        /// <remarks>
        /// NOTE: with async func, use semaphore, never use "lock" statement :
        /// SOURCE : https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
        /// </remarks>
        private SemaphoreSlim m_DictionaryLocker = new SemaphoreSlim(1, 1);

        /// <summary>
        /// collect cache with type
        /// collect assembly of type
        /// collect all type in assembly
        /// </summary>
        internal IEnumerable<Type> CollectWithCache(Type p_Type)
        {
            TryPopulateAssemblyCache(p_Type);
            return CollectWithCache(m_TypeToAssembly[p_Type]);
        }

        protected override IEnumerable<Type> CollectToPopulateCache(Assembly p_Filter)
        {
            return p_Filter.GetExportedTypes().Where(p_Types => IsValidResult(p_Types));
        }

        private void TryPopulateAssemblyCache(Type p_Type)
        {
            m_DictionaryLocker.WaitFor(millisecondsTimeout: 1500, action: () =>
            {
                if (m_TypeToAssembly.ContainsKey(p_Type) == false)
                    m_TypeToAssembly[p_Type] = p_Type.Assembly;
            });
        }
    }
}