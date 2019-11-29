using System;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    public struct TypeParam
    {
        public TypeParam(Type p_Type, string p_Value, params Type[] p_Params)
        {
            Type = p_Type;
            Params = p_Params;
            Value = p_Value;
        }

        public Type Type { get; set; }

        public string Value { get; set; }

        public Type[] Params { get; set; }
    }
}