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
        public static string GetDescription(this Enum value)
        {
            var v_EnumValue = value.GetType().GetField(value.ToString());

            var attr = v_EnumValue.GetCustomAttribute<DescriptionAttribute>(inherit: false);
            if (attr == null)
                return value.ToString();

            return attr.Description;
        }

        /// <summary>
        /// try to collect enum value with same description
        /// </summary>
        /// <return>default value if description not found</return>
        public static TEnum GetEnumOrDefaultByDescription<TEnum>(this string wantedStr) where TEnum : Enum
        {
            return GetAllEnumByDescription<TEnum>(wantedStr)
                                                .FirstOrDefault();
        }

        /// <summary>
        /// try to collect enum value with same description
        /// </summary>
        public static bool TryGetEnumByDescription<T>(this string wantedStr, out T result) where T : Enum
        {
            try
            {
                result = GetAllEnumByDescription<T>(wantedStr)
                                                            .First();

                return true;
            }
            catch (InvalidOperationException)
            {
                result = default(T);
                return false;
            }
        }

        private static IEnumerable<T> GetAllEnumByDescription<T>(string wantedStr) where T : Enum
        {
            return Enum.GetValues(typeof(T))
                        .OfType<T>()
                        .Where(p_Value => GetDescription(p_Value) == wantedStr);
        }
    }
}