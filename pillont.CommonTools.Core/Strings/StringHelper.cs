using System.Collections.Generic;
using System.Linq;

namespace pillont.CommonTools.Core.Strings
{
    public static class StringHelper
    {
        /// <summary>
        /// transform text to simla camel text 
        /// only one upper letter
        /// </summary>
        /// <example>
        /// camelcase => Camelcase
        /// Camelcase => Camelcase
        /// CamelCase => Camelcase
        /// </example>
        public static string ToSimpleCamel(this string value)
        {
            return new string(
                                
                                new List<char>(value[0].ToString().ToUpper())           // NOTE : first in upper
                                        .Concat(value.Substring(1, value.Length - 1)    // NOTE : other in lower
                                                     .ToLower())                        
                                        .ToArray());
        }
    }
}
