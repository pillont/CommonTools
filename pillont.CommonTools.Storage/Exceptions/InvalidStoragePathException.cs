using System;

namespace pillont.CommonTools.Storage.Exceptions
{
    /// <summary>
    /// Exception thrown when storage path is invalid
    /// </summary>
    public class InvalidStoragePathException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="p_ActionName">Name of the caller</param>
        /// <param name="p_Path">Path causing the issue</param>
        /// <param name="p_InnerException">Catched inner exception</param>
        public InvalidStoragePathException(string p_ActionName, string p_Path, Exception p_InnerException) : base($"An error happened while trying to {p_ActionName} the following item : {p_Path}. See the inner Exception for more details.", p_InnerException)
        {
        }
    }
}