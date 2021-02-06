using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pillont.CommonTools.Core.Reflection.ReflectionCaches
{
    /// <summary>
    /// cache to collect all properties of a type
    /// </summary>
    public class PropertiesCache : ListReflectionCache<Type, PropertyInfo>
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
        private readonly bool m_AllProperties;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="p_AllProperty">
        /// inform if must check on all properties of the type
        /// use open rules by <see cref="BindingFlags"/> during collect
        /// <seealso cref="ReflexionTool.GetPropertiesOfType" />
        /// </param>
        public PropertiesCache(bool p_AllProperty = true)
        {
            m_AllProperties = p_AllProperty;
        }

        public PropertyInfo GetInCache(Type p_Type, string p_FieldName)
        {
            return this.CollectWithCache(p_Type)?.SingleOrDefault(c => c.Name == p_FieldName);
        }

        /// <summary>
        ///  collect all properties in the type
        ///  filter only valid property
        /// </summary>
        /// <returns>empty list if type not have property</returns>
        /// <seealso cref="PropertyInfo"/>
        protected sealed override IEnumerable<PropertyInfo> CollectToPopulateCache(Type p_Type)
        {
            PropertyInfo[] v_PropertyInfo = m_AllProperties
                    ? p_Type.GetProperties(AllPropBindingFlags)
                    : p_Type.GetProperties();

            return v_PropertyInfo.Where(p_Property => IsValidResult(p_Property));
        }
    }
}