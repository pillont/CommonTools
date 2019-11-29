using System;

namespace pillont.CommonTools.Core.Security
{
    public class HashService : IHashService
    {
        /// <summary>
        /// type used to hash value
        /// </summary>
        public HashType HashType { get; }

        /// <summary>
        /// inform if the result is in base 64 or not
        /// </summary>
        public bool UseBase64 { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="p_HashType"> <see cref="HashType"/> </param>
        public HashService(HashType p_HashType)
        {
            HashType = p_HashType;
        }

        /// <summary>
        /// use <see cref="HashType"/> and <see cref="Key"/> to hash value
        /// </summary>
        /// <returns> if <see cref="UseBase64"/> is true : result is in base 64 </returns>
        /// <exception cref="InvalidOperationException"> hash type not found </exception>
        public virtual bool CheckValue(string p_Original, string p_Hashed, string p_Key = null)
        {
            return UseBase64
                ? HashHelper.CheckHashBase64(p_Original, p_Hashed, HashType, p_Key)
                : HashHelper.CheckHash(p_Original, p_Hashed, HashType, p_Key);
        }

        /// <summary>
        /// compare with hased value with <see cref="HashType"/> and <see cref="Key"/>
        /// if <see cref="UseBase64"/> is true : compare value is in base 64
        /// </summary>
        /// <returns> infom if is associated value or not </returns>
        /// <exception cref="InvalidOperationException"> hash type not found </exception>
        public virtual string HashValue(string p_Value, string p_Key = null)
        {
            return UseBase64
                ? HashHelper.GetHashBase64(p_Value, HashType, p_Key)
                : HashHelper.GetHash(p_Value, HashType, p_Key);
        }

        /// <summary>
        /// transform key array in string and hash values
        /// <see cref="HashValue(string, string)"/>
        /// </summary>
        /// <param name="p_Value"></param>
        /// <param name="p_Key"></param>
        /// <returns></returns>
        public virtual string HashValue(string p_Value, char[] p_Key)
        {
            string keyStr = new string(p_Key);
            return HashValue(p_Value, keyStr);
        }
    }
}