using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pillont.CommonTools.Core.Parallel;
using pillont.CommonTools.Storage.Abstractions;

namespace pillont.CommonTools.Storage.FileSystem
{
    /// <summary>
    /// File storage provider handling concurrent requests
    /// </summary>
    public class ConcurrentFileSystemStorageProvider : IAsyncStorageProvider
    {
        /// <summary>
        /// Exception message template
        /// </summary>
        private static readonly string ASYNC_EXCEPTION_MESSAGE = $"Async operation failed in {{0}} (Assembly : {typeof(ConcurrentFileSystemStorageProvider).AssemblyQualifiedName}, Class {nameof(ConcurrentFileSystemStorageProvider)})";

        /// <summary>
        /// File storage provider
        /// </summary>
        private FileSystemStorageProvider Storage { get; }

        /// <summary>
        /// Locks for concurrent accesses
        /// </summary>
        private MultiLock<string> Locks { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public ConcurrentFileSystemStorageProvider()
        {
            Storage = new FileSystemStorageProvider();
            Locks = new MultiLock<string>(1);
        }

        #region Implementation

        /// <summary>
        /// Delete a file in a particular path
        /// </summary>
        /// <param name="p_Path">Path of the file to delete</param>
        public virtual void Delete(string p_Path)
        {
            WaitForPath(p_Path, p_Action: () =>
            {
                Storage.Delete(p_Path);
            });
        }

        /// <summary>
        /// Delete a file in a particular path
        /// </summary>
        /// <param name="p_Path">Path of the file to delete</param>
        public virtual async Task DeleteAsync(string p_Path)
        {
            await WaitForPathAsync(p_Path, p_Action: async () =>
            {
                await Storage.DeleteAsync(p_Path);
            });
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>Does the file exist</returns>
        public virtual bool FileExists(string p_Path)
        {
            return WaitForPath(p_Path, p_GenericAction: () =>
            {
                return Storage.FileExists(p_Path);
            });
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>Does the file exist</returns>
        public virtual async Task<bool> FileExistsAsync(string p_Path)
        {
            return await WaitForPathAsync(p_Path, p_GenericAction: async () =>
            {
                return await Storage.FileExistsAsync(p_Path);
            });
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual async Task<byte[]> GetByteAsync(string p_Path)
        {
            return await WaitForPathAsync(p_Path, p_GenericAction: async () =>
            {
                return await Storage.GetByteAsync(p_Path);
            });
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual byte[] GetByte(string p_Path)
        {
            return WaitForPath(p_Path, p_GenericAction: () =>
            {
                return Storage.GetByte(p_Path);
            });
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual string GetString(string p_Path)
        {
            return WaitForPath(p_Path, p_GenericAction: () =>
            {
                return Storage.GetString(p_Path);
            });
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual async Task<string> GetStringAsync(string p_Path)
        {
            return await WaitForPathAsync(p_Path, p_GenericAction: async () =>
            {
                return await Storage.GetStringAsync(p_Path);
            });
        }

        /// <summary>
        /// Search for a given pattern in the given path, recursivly or not
        /// </summary>
        /// <param name="p_Path">Path to search in</param>
        /// <param name="p_Pattern">Pattern to search for</param>
        /// <param name="p_Recurse">Recursive search</param>
        /// <param name="p_Predicate">Predicate for other manipulations</param>
        /// <returns>List of files matching the pattern</returns>
        public virtual async Task<IEnumerable<string>> SearchAsync(string p_Path, string p_Pattern, bool p_Recurse = false, Func<IEnumerable<string>, IEnumerable<string>> p_Predicate = null)
        {
            return await Storage.SearchAsync(p_Path, p_Pattern, p_Recurse, p_Predicate);
        }

        /// <summary>
        /// Search for a given pattern in the given path, recursivly or not
        /// </summary>
        /// <param name="p_Path">Path to search in</param>
        /// <param name="p_Pattern">Pattern to search for</param>
        /// <param name="p_Recurse">Recursive search</param>
        /// <param name="p_Predicate">Predicate for other manipulations</param>
        /// <returns>List of files matching the pattern</returns>
        public virtual IEnumerable<string> Search(string p_Path, string p_Pattern, bool p_Recurse = false, Func<IEnumerable<string>, IEnumerable<string>> p_Predicate = null)
        {
            return Storage.Search(p_Path, p_Pattern, p_Recurse, p_Predicate);
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_Content">Content to store</param>
        public virtual void Store(string p_Path, string p_Content)
        {
            WaitForPath(p_Path, p_Action: () =>
            {
                Storage.Store(p_Path, p_Content);
            });
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_Content">Content to store</param>
        public virtual async Task StoreAsync(string p_Path, string p_Content)
        {
            await WaitForPathAsync(p_Path, p_Action: async () =>
            {
                await Storage.StoreAsync(p_Path, p_Content);
            });
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_ByteContent">Byte content to store</param>
        public virtual async Task StoreAsync(string p_Path, byte[] p_ByteContent)
        {
            await WaitForPathAsync(p_Path, p_Action: async () =>
            {
                await Storage.StoreAsync(p_Path, p_ByteContent);
            });
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_ByteContent">Byte content to store</param>
        public virtual void Store(string p_Path, byte[] p_ByteContent)
        {
            WaitForPath(p_Path, p_Action: () =>
            {
                Storage.Store(p_Path, p_ByteContent);
            });
        }

        #endregion Implementation

        #region Wait For Path

        /// <summary>
        /// Wait for the current path for being available
        /// </summary>
        /// <param name="p_Path">Path to watch</param>
        /// <param name="p_Action">Action to execute</param>
        private void WaitForPath(string p_Path, Action p_Action)
        {
            var v_Lock = Locks.GetCurrentLock(p_Path);
            bool success = v_Lock.WaitFor(p_Action);

            if (!success)
                throw new InvalidOperationException(String.Format(ASYNC_EXCEPTION_MESSAGE, nameof(WaitForPath)));
        }

        /// <summary>
        /// Wait for the current path for being available
        /// </summary>
        /// <param name="p_Path">Path to watch</param>
        /// <param name="p_GenericAction">Action to execute</param>
        private T WaitForPath<T>(string p_Path, Func<T> p_GenericAction)
        {
            T result = default;
            WaitForPath(p_Path, p_Action: () => result = p_GenericAction());
            return result;
        }

        /// <summary>
        /// Wait for the current path for being available
        /// </summary>
        /// <param name="p_Path">Path to watch</param>
        /// <param name="p_GenericAction">Action to execute</param>
        private async Task<T> WaitForPathAsync<T>(string p_Path, Func<Task<T>> p_GenericAction)
        {
            T result = default;
            await WaitForPathAsync(p_Path, p_Action: async () => result = await p_GenericAction());
            return result;
        }

        /// <summary>
        /// Wait for the current path for being available
        /// </summary>
        /// <param name="p_Path">Path to watch</param>
        /// <param name="p_Action">Action to execute</param>
        private async Task WaitForPathAsync(string p_Path, Func<Task> p_Action)
        {
            SemaphoreSlim v_Lock = Locks.GetCurrentLock(p_Path);
            bool success = await v_Lock.WaitForAsync(p_Action);

            if (!success)
                throw new InvalidOperationException(String.Format(ASYNC_EXCEPTION_MESSAGE, nameof(WaitForPathAsync)));
        }

        #endregion Wait For Path
    }
}