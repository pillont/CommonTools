using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pillont.CommonTools.Core.Reflection.ReflectionCaches
{
    /// <summary>
    /// cache to collect all fields of a type
    /// </summary>
    public class FieldsCache : ListReflectionCache<Type, FieldInfo>
    {
        public FieldInfo GetInCache(Type p_Type, string p_FieldName)
        {
            return this.CollectWithCache(p_Type)?.SingleOrDefault(c => c.Name == p_FieldName);
        }

        /// <summary>
        ///  collect all properties in the type
        ///  filter only valid property
        /// </summary>
        /// <returns>empty list if type not have property</returns>
        /// <seealso cref="PropertyInfo"/>
        protected sealed override IEnumerable<FieldInfo> CollectToPopulateCache(Type p_Type)
        {
            return p_Type.GetFields().Where(p_Field => IsValidResult(p_Field));
        }
    }
}