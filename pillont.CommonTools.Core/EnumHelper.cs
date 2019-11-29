using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace pillont.CommonTools.Core
{
    public static class EnumHelper
    {
        /// <summary>
        /// collect description of enum value
        /// get value field
        /// collect wanted attribute
        /// return string value
        /// </summary>
        public static string GetDescription<TAttri>(this Enum value)
            where TAttri : Attribute
        {
            var v_EnumValue = value.GetType().GetField(value.ToString());

            var attr = v_EnumValue.GetCustomAttribute<DescriptionAttribute>(inherit: false);
            if (attr == null)
                return value.ToString();

            return attr.Description;
        }

        public static string GetDescription(this Enum value)
        {
            return GetDescription<DescriptionAttribute>(value);
        }

        public static TEnum GetEnumOrDefaultByDescription<TEnum>(this string wantedStr)
            where TEnum : Enum
        {
            return GetEnumOrDefaultByDescription<TEnum, DescriptionAttribute>(wantedStr);
        }

        /// <summary>
        /// try to collect enum value with same description
        /// </summary>
        /// <return>default value if description not found</return>
        public static TEnum GetEnumOrDefaultByDescription<TEnum, TAttri>(this string wantedStr) where TEnum : Enum
            where TAttri : Attribute
        {
            return GetAllEnumByDescription<TEnum, TAttri>(wantedStr)
                                                .FirstOrDefault();
        }

        public static bool TryGetEnumByDescription<T>(this string wantedStr, out T result)
            where T : Enum
        {
            return TryGetEnumByDescription<T, DescriptionAttribute>(wantedStr, out result);
        }

        /// <summary>
        /// try to collect enum value with same description
        /// </summary>
        public static bool TryGetEnumByDescription<T, TAttri>(this string wantedStr, out T result) where T : Enum
            where TAttri : Attribute
        {
            try
            {
                result = GetAllEnumByDescription<T, TAttri>(wantedStr)
                                                            .First();

                return true;
            }
            catch (InvalidOperationException)
            {
                result = default(T);
                return false;
            }
        }

        private static IEnumerable<T> GetAllEnumByDescription<T, TAttri>(string wantedStr) where T : Enum
            where TAttri : Attribute
        {
            return Enum.GetValues(typeof(T))
                        .OfType<T>()
                        .Where(p_Value => GetDescription<TAttri>(p_Value) == wantedStr);
        }
    }
}