namespace pillont.CommonTools.Core.Security
{
    /// <summary>
    /// service to hash string value
    /// </summary>
    public interface IHashService
    {
        /// <summary>
        /// inform if value is associated to the hashed value
        /// </summary>
        bool CheckValue(string p_Original, string p_Hashed, string p_Key = null);

        string HashValue(string p_Value, string p_Key = null);

        string HashValue(string p_Value, char[] p_Key);
    }
}
