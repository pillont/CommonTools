using System;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    public class GenericTypeDefinitionCache : BaseReflectionCache<Type, Type>
    {
        protected override Type CollectToPopulateCache(Type p_Type)
        {
            return p_Type.GetGenericTypeDefinition();
        }
    }
}