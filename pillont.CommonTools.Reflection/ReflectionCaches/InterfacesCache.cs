using System;
using System.Collections.Generic;
using System.Linq;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    internal class InterfacesCache : ListReflectionCache<Type, Type>
    {
        protected override IEnumerable<Type> CollectToPopulateCache(Type p_Type)
        {
            return p_Type.GetInterfaces().Where(p_Interface => IsValidResult(p_Interface));
        }
    }
}