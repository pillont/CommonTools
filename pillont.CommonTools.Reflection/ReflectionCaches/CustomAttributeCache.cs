using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    public class CustomAttributeCache : ListReflectionCache<MemberInfo, object>
    {
        protected override IEnumerable<object> CollectToPopulateCache(MemberInfo p_Type)
        {
            return p_Type.GetCustomAttributes(true).Where(p_CustomArgument => IsValidResult(p_CustomArgument));
        }
    }
}