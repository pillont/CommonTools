using System;
using System.Collections.Generic;

namespace pillont.CommonTools.Storage.Abstractions
{
    /// <summary>
    /// Storage provider interface
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Delete an item at a specifi path in a storage
        /// </summary>
        /// <param name="p_Path">Item to delete</param>
        void Delete(string p_Path);

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>Does the file exist</returns>
        bool FileExists(string p_Path);

        /// <summary>
        /// Get the content (as string) of a stored item
        /// </summary>
        /// <param name="p_Path"></param>
        /// <returns></returns>
        byte[] GetByte(string p_Path);

        /// <summary>
        /// Get the content (as string) of a stored item
        /// </summary>
        /// <param name="p_Path"></param>
        /// <returns></returns>
        string GetString(string p_Path);

        /// <summary>
        /// Search for a given pattern in the given path, recursivly or not
        /// </summary>
        /// <param name="p_Path">Path to search in</param>
        /// <param name="p_Pattern">Pattern to search for</param>
        /// <param name="p_Recurse">Recursive search</param>
        /// <param name="p_FilterAction"> filter applied on result </param>
        /// <returns>List of files matching the pattern</returns>
        IEnumerable<string> Search(string p_Path, string p_Pattern, bool p_Recurse = false, Func<IEnumerable<string>, IEnumerable<string>> p_FilterAction = null);

        /// <summary>
        /// Store a content in a specific path on the storage
        /// </summary>
        /// <param name="p_Path">Path where to store item</param>
        /// <param name="p_Content">Content to store</param>
        void Store(string p_Path, string p_Content);

        /// <summary>
        /// Store a content in a specific path on the storage
        /// </summary>
        /// <param name="p_Path">Path where to store item</param>
        /// <param name="p_ByteContent">Byte content to store</param>
        /// <returns></returns>
        void Store(string p_Path, byte[] p_ByteContent);
    }
}
