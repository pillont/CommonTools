using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pillont.CommonTools.Core.Security
{
    /// <summary>
    /// helper to hash value with <see cref="HashType"/> method
    /// </summary>
    internal class HashHelper
    {
        /// <summary>Compare un text avec un hash</summary>
        /// <param name="p_originalString">Text a comparé avec le hash</param>
        /// <param name="p_hashedString">Le hash qui va servir a la comparaison</param>
        /// <param name="p_hashType">Type de hash</param>
        /// <param name="p_key">[Optional] key for the hash</param>
        /// <returns>Retourne si la comparaison du hash est bonne ou sinon false</returns>
        public static bool CheckHash(string p_originalString, string p_hashedString, HashType p_hashType, string p_key = null)
        {
            string strOrigHash = GetHash(p_originalString, p_hashType, p_key);
            return (strOrigHash.Equals(p_hashedString));
        }

        /// <summary>Compare un text avec un hash base64</summary>
        /// <param name="p_originalString">Text a comparé avec le hash base64</param>
        /// <param name="p_hashedString">Le hash base64 qui va servir a la comparaison</param>
        /// <param name="p_hashType">Type de hash</param>
        /// <param name="p_key">[Optional] key for the hash</param>
        /// <returns>Retourne si la comparaison du hash est bonne ou sinon false</returns>
        public static bool CheckHashBase64(string p_originalString, string p_hashedString, HashType p_hashType, string p_key = null)
        {
            string strOrigHash = GetHashBase64(p_originalString, p_hashType, p_key);
            return (strOrigHash.Equals(p_hashedString));
        }

        /// <summary>Génére le hash d'un text</summary>
        /// <param name="p_plainText">Text que l'on désire obtenir le hash</param>
        /// <param name="p_hashType">Type de Hashage</param>
        /// <param name="p_key">[Optional] key for the hash</param>
        /// <returns>le hash obtenu</returns>
        public static string GetHash(string p_plainText, HashType p_hashType, string p_key = null)
        {
            if (String.IsNullOrEmpty(p_plainText))
                return string.Empty;

            HashAlgorithm v_Algo = GetHashAlgorithm(p_hashType, p_key);
            return GetFinalString(GetHashValue(p_plainText, v_Algo));
        } /* GetHash */

        /// <summary>
        /// Génére le hash d'un text
        /// </summary>
        /// <param name="p_plainText">Text que l'on désire obtenir le hash base64</param>
        /// <param name="p_hashType">Type de Hashage</param>
        /// <param name="p_key">[Optional] key for the hash</param>
        /// <returns>Hash au format base64</returns>
        public static string GetHashBase64(string p_plainText, HashType p_hashType, string p_key = null)
        {
            if (String.IsNullOrEmpty(p_plainText))
                return string.Empty;

            HashAlgorithm v_Algo = GetHashAlgorithm(p_hashType, p_key);
            return Convert.ToBase64String(GetHashValue(p_plainText, v_Algo));
        }

        /* GetHash */

        /* CheckHash */

        /* CheckHash */

        /// <summary>
        /// Get bytes array from a string
        /// </summary>
        /// <param name="p_StringToHash">String to hash</param>
        /// <returns>Bytes array</returns>
        private static byte[] GetBytes(string p_StringToHash)
        {
            if (String.IsNullOrEmpty(p_StringToHash))
                throw new ArgumentNullException(nameof(p_StringToHash), "The string cannot be null or empty");

            return Encoding.ASCII.GetBytes(p_StringToHash);
        }

        /// <summary>
        /// Get the final Script after the encoded from the HashValue
        /// </summary>
        /// <param name="p_hashValue">ComputeHash from the HashAlgorithm</param>
        /// <returns></returns>
        private static string GetFinalString(byte[] p_hashValue)
        {
            if (p_hashValue == null)
                throw new ArgumentNullException(nameof(p_hashValue), "The encoded hashValue cannot be null to be convert into string");

            StringBuilder strHex = new StringBuilder();
            try
            {
                p_hashValue.ToList().ForEach(o => strHex.AppendFormat("{0:x2}", o));
                return strHex.ToString();
            }
            finally
            {
                strHex.Clear();
                strHex = null;
            }
        }

        /// <summary>
        /// Get the correct HashAlgorithm, depends if you need a key for the hash or not
        /// </summary>
        /// <param name="p_hashType">Type of Hash</param>
        /// <param name="p_key">Key to use for the Hash</param>
        /// <returns></returns>
        private static HashAlgorithm GetHashAlgorithm(HashType p_hashType, string p_key = null)
        {
            HashAlgorithm SHhash = null;
            switch (p_hashType)
            {
                case HashType.MD5:
                    if (String.IsNullOrEmpty(p_key))
                        SHhash = MD5.Create();
                    else
                        SHhash = new HMACMD5(GetBytes(p_key));
                    break;

                case HashType.SHA1:
                    if (String.IsNullOrEmpty(p_key))
                        SHhash = SHA1.Create();
                    else
                        SHhash = new HMACSHA1(GetBytes(p_key));
                    break;

                case HashType.SHA256:
                    if (String.IsNullOrEmpty(p_key))
                        SHhash = SHA256.Create();
                    else
                        SHhash = new HMACSHA256(GetBytes(p_key));
                    break;

                case HashType.SHA384:
                    if (String.IsNullOrEmpty(p_key))
                        SHhash = SHA384.Create();
                    else
                        SHhash = new HMACSHA384(GetBytes(p_key));
                    break;

                case HashType.SHA512:
                    if (String.IsNullOrEmpty(p_key))
                        SHhash = SHA512.Create();
                    else
                        SHhash = new HMACSHA512(GetBytes(p_key));
                    break;

                default:
                    throw new InvalidOperationException($"Unknown hash type \"{p_hashType}\"");
            }

            return SHhash;
        }

        /// <summary>
        /// Get the hash value from the HashAlgorithm
        /// </summary>
        /// <param name="p_plainText">String to hash</param>
        /// <param name="p_hashClass">HashAlgorithm class to use (MD5, SHA512)</param>
        /// <returns>Byte array</returns>
        private static byte[] GetHashValue(string p_plainText, HashAlgorithm p_hashClass)
        {
            if (String.IsNullOrEmpty(p_plainText))
                throw new ArgumentNullException(nameof(p_plainText), "The string to hash cannot be null or empty");

            if (p_hashClass == null)
                throw new ArgumentNullException(nameof(p_hashClass), "HashAlgorithm class cannot be null");

            byte[] v_MessageBytes = null;
            try
            {
                v_MessageBytes = GetBytes(p_plainText);
                return p_hashClass.ComputeHash(v_MessageBytes);
            }
            finally
            {
                v_MessageBytes = null;
            }
        }
    }
}