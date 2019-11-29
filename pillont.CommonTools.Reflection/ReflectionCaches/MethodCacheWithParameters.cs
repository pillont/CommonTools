using System;
using System.Reflection;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    public class MethodCacheWithParameters : BaseReflectionCache<TypeParam, MethodInfo>
    {
        /// <summary>
        ///  collect all properties in the type
        ///  filter only valid property
        /// </summary>
        /// <returns>empty list if type not have property</returns>
        /// <seealso cref="MethodInfo"/>
        protected  override MethodInfo CollectToPopulateCache(TypeParam p_Type)
        {
            try
            {
                return p_Type.Type.GetMethod(p_Type.Value, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
            }
            catch (AmbiguousMatchException)
            {
                return p_Type.Type.GetMethod(p_Type.Value, p_Type.Params);
            }
        }

        public MethodInfo CollectWithCache(Type p_Type, string p_Value, params Type[] p_Params)
        {
            return CollectWithCache(new TypeParam(p_Type, p_Value, p_Params));
        }
    }
}