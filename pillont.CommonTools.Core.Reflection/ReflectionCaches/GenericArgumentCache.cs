using System;
using System.Collections.Generic;
using System.Linq;

namespace pillont.CommonTools.Core.Reflection.ReflectionCaches
{
    public class GenericArgumentsCache : ListReflectionCache<Type, Type>
    {
        protected override IEnumerable<Type> CollectToPopulateCache(Type p_Type)
        {
            return p_Type.GetGenericArguments().Where(p_Argument => IsValidResult(p_Argument));
        }
    }
}