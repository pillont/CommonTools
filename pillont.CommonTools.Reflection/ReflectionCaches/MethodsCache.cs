using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    public class MethodsCache : ListReflectionCache<Type, MethodInfo>
    {
        /// <summary>
        /// binding flags to collect all properties
        /// </summary>
        private const BindingFlags AllPropBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase;

        /// <summary>
        /// inform if must check on all properties of the type
        /// use open rules by <see cref="BindingFlags"/> during collect
        /// <seealso cref="ReflexionTool.GetPropertiesOfType" />
        /// <seealso cref="AllPropBindingFlags"/>
        /// </summary>
        private readonly bool m_AllMethods;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="p_AllProperty">
        /// inform if must check on all properties of the type
        /// use open rules by <see cref="BindingFlags"/> during collect
        /// <seealso cref="ReflexionTool.GetPropertiesOfType" />
        /// </param>
        public MethodsCache(bool p_AllProperty = true)
        {
            m_AllMethods = p_AllProperty;
        }

        /// <summary>
        ///  collect all properties in the type
        ///  filter only valid property
        /// </summary>
        /// <returns>empty list if type not have property</returns>
        /// <seealso cref="MethodInfo"/>
        protected  override IEnumerable<MethodInfo> CollectToPopulateCache(Type p_Type)
        {
            MethodInfo[] v_MethodInfo = m_AllMethods
                    ? p_Type.GetMethods(AllPropBindingFlags)
                    : p_Type.GetMethods();

            return v_MethodInfo.Where(p_MethodInfo => IsValidResult(p_MethodInfo));
        }
    }
}