using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pillont.CommonTools.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert HTML text to plain text.
        /// </summary>
        /// <param name="p_HTMLText">Html Text</param>
        /// <returns></returns>
        public static string HTMLToText(this string p_HTMLText)
        {
            StringBuilder v_sbHTML = null;
            string[] v_oldWords = null;
            string[] v_newWords = null;
            try
            {
                // Remove new lines since they are not visible in HTML
                p_HTMLText = p_HTMLText.Replace("\n", " ");

                // Remove tab spaces
                p_HTMLText = p_HTMLText.Replace("\t", " ");

                // Remove multiple white spaces from HTML
                p_HTMLText = Regex.Replace(p_HTMLText, "\\s+", " ");

                // Remove HEAD tag
                p_HTMLText = Regex.Replace(p_HTMLText, "<head.*?</head>", ""
                                    , RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // Remove any JavaScript
                p_HTMLText = Regex.Replace(p_HTMLText, "<script.*?</script>", ""
                  , RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // Replace special characters like &, <, >, " etc.
                v_sbHTML = new StringBuilder(p_HTMLText);
                // Note: There are many more special characters, these are just
                // most common. You can add new characters in this arrays if needed
                v_oldWords = new string[] { "&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;", "&reg;", "&copy;", "&bull;", "&trade;" };
                v_newWords = new string[] { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢" };
                for (int i = 0; i < v_oldWords.Length; i++)
                {
                    v_sbHTML.Replace(v_oldWords[i], v_newWords[i]);
                }

                // Check if there are line breaks (<br>) or paragraph (<p>)
                v_sbHTML.Replace("<br>", "\n<br>");
                v_sbHTML.Replace("<br ", "\n<br ");
                v_sbHTML.Replace("<p ", "\n<p ");

                // Finally, remove all HTML tags and return plain text
                return System.Text.RegularExpressions.Regex.Replace(
                  v_sbHTML.ToString(), "<[^>]*>", "");
            }
            finally
            {
                v_sbHTML = null;
                v_oldWords = null;
                v_newWords = null;
            }
        }

        /// <summary>
        /// Get Base64 string
        /// </summary>
        /// <param name="p_value">Clear string</param>
        /// <returns>Base64 string</returns>
        public static string EncodeToBase64(this string p_value)
        {
            if (string.IsNullOrEmpty(p_value))
                throw new ArgumentNullException(nameof(p_value), "String value cannot be null or empty");

            var v_PlainTextBytes = System.Text.Encoding.UTF8.GetBytes(p_value);
            return System.Convert.ToBase64String(v_PlainTextBytes);
        }

        /// <summary>
        /// Get clear string from Base64 string
        /// </summary>
        /// <param name="p_value">Base64 string</param>
        /// <returns>Clear string</returns>
        public static string DecodeFromBase64(this string p_value)
        {
            if (string.IsNullOrEmpty(p_value))
                throw new ArgumentNullException(nameof(p_value), "String value cannot be null or empty");

            var v_Base64EncodedBytes = System.Convert.FromBase64String(p_value);
            return System.Text.Encoding.UTF8.GetString(v_Base64EncodedBytes);
        }

        /// <summary>
        /// Get Bytes from string in Unicode Encoding
        /// </summary>
        /// <remarks>Use Unicode Encoding</remarks>
        /// <param name="p_value">value to convert</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string p_value)
        {
            if (string.IsNullOrEmpty(p_value))
                throw new ArgumentNullException(nameof(p_value), "String value cannot be null or empty");

            return Encoding.Unicode.GetBytes(p_value);
        }

        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="p_value">Byte array</param>
        /// <returns>String value</returns>
        public static string GetStringFromBytes(this byte[] p_value)
        {
            if (p_value == null)
                throw new ArgumentNullException(nameof(p_value), "Byte array cannot be null");

            return Encoding.Unicode.GetString(p_value);
        }

        /// <summary>
        /// Permet de supprimer les caracteres speciaux
        /// Il suffit de s'appuyer sur la page de code 1251 dans lequel les caracteres diacritiques
        /// sont codés sur 2 octets, avec un octet pour le caractere de base et un octet
        /// pour la variante ( e -> e, é, e, e, ë ...)
        /// En repassant en ascii, on ne garde que le caractere de base*/
        /// </summary>
        /// <param name="p_input">Remplace les caracteres accentues, nordique, iberique</param>
        /// <param name="p_encodings">List d'encodings a utiliser ex : 1251, 1252 et 1257</param>
        /// <returns>Sanitized text</returns>
        public static String RemoveSpecialChars(this string p_input, List<int> p_encodings)
        {
            string v_text = p_input;
            byte[] v_bytes = null;

            foreach (int v_encoding in p_encodings)
            {
                v_bytes = Encoding.GetEncoding(v_encoding).GetBytes(v_text);
                v_text = Encoding.ASCII.GetString(v_bytes);
            }
            return v_text;
        }

        /// <summary>
        /// Capitalize the first character of String
        /// </summary>
        /// <param name="p_string">String to transform</param>
        /// <returns>First letter capitalized string</returns>
        public static string UppercaseFirst(this string p_string)
        {
            // Check for empty string.
            if (String.IsNullOrEmpty(p_string))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return $"{char.ToUpper(p_string[0])}{p_string.Substring(1).ToLower()}";
        }

        /// <summary>
        /// Pascalize a string
        /// </summary>
        /// <param name="p_text">Text to pascalize</param>
        /// <returns>Pascalized text</returns>
        public static string ToPascalCase(this string p_text)
        {
            // TODO: Better regex for handling some cases
            return Regex.Replace(p_text, @"^\w|\s\w", (match) => match.Value.Replace(" ", "").ToUpper());
        }
    }
}