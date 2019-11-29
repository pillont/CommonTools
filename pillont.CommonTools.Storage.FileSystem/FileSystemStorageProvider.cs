using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using pillont.CommonTools.Storage.Abstractions;
using pillont.CommonTools.Storage.Exceptions;

namespace pillont.CommonTools.Storage.FileSystem
{
    /// <summary>
    /// Storage provider for the file system
    /// </summary>
    public class FileSystemStorageProvider : IAsyncStorageProvider
    {
        /// <summary>
        /// Delete a file in a particular path
        /// </summary>
        /// <param name="p_Path">Path of the file to delete</param>
        public virtual void Delete(string p_Path)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            try
            {
                File.Delete(p_Path);
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(Delete).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Delete a file in a particular path
        /// </summary>
        /// <param name="p_Path">Path of the file to delete</param>
        public virtual Task DeleteAsync(string p_Path)
        {
            return Task.Run(() => Delete(p_Path));
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>Does the file exist</returns>
        public virtual bool FileExists(string p_Path)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path));

            try
            {
                return File.Exists(p_Path);
            }
            catch (Exception ex) when (ex is PathTooLongException)
            {
                throw new InvalidStoragePathException(nameof(FileExists).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>Does the file exist</returns>
        public virtual Task<bool> FileExistsAsync(string p_Path)
        {
            return Task.Run(() => FileExists(p_Path));
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual byte[] GetByte(string p_Path)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            try
            {
                return File.ReadAllBytes(p_Path);
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(GetByte).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual async Task<byte[]> GetByteAsync(string p_Path)
        {
            return await Task.Run(() =>
            {
                return GetByte(p_Path);
            });
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual string GetString(string p_Path)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            try
            {
                return File.ReadAllText(p_Path);
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(GetString).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Get the content of a particular file
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <returns>File content</returns>
        public virtual async Task<string> GetStringAsync(string p_Path)
        {
            return await Task.Run(() =>
            {
                return GetString(p_Path);
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
        public virtual IEnumerable<string> Search(string p_Path, string p_Pattern, bool p_Recurse = false, Func<IEnumerable<string>, IEnumerable<string>> p_Predicate = null)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            if (String.IsNullOrWhiteSpace(p_Pattern))
                throw new ArgumentNullException(nameof(p_Pattern), "The pattern must be provided.");

            try
            {
                IEnumerable<string> v_Result = Directory.GetFiles(p_Path, p_Pattern, p_Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                if (p_Predicate != null)
                {
                    v_Result = p_Predicate(v_Result);
                }

                return v_Result;
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(Search).ToLower(), p_Path, ex);
            }
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
            return await Task.Run(() =>
            {
                return Search(p_Path, p_Pattern, p_Recurse, p_Predicate);
            });
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_Content">Content to store</param>
        public virtual void Store(string p_Path, string p_Content)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            try
            {
                CreateDirectories(p_Path);

                File.WriteAllText(p_Path, p_Content);
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(Store).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_ByteContent">Byte content to store</param>
        public void Store(string p_Path, byte[] p_ByteContent)
        {
            if (String.IsNullOrWhiteSpace(p_Path))
                throw new ArgumentNullException(nameof(p_Path), "The provided path cannot be null or whitespace.");

            try
            {
                CreateDirectories(p_Path);

                File.WriteAllBytes(p_Path, p_ByteContent);
            }
            catch (Exception ex) when (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new InvalidStoragePathException(nameof(Store).ToLower(), p_Path, ex);
            }
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_Content">Content to store</param>
        public virtual Task StoreAsync(string p_Path, string p_Content)
        {
            return Task.Run(() =>
            {
                Store(p_Path, p_Content);
            });
        }

        /// <summary>
        /// Store the content in a particular file
        /// </summary>
        /// <param name="p_Path">File path to store the content (the content will be overriden)</param>
        /// <param name="p_ByteContent">Byte content to store</param>
        public Task StoreAsync(string p_Path, byte[] p_ByteContent)
        {
            return Task.Run(() =>
            {
                Store(p_Path, p_ByteContent);
            });
        }

        /// <summary>
        /// Create all non-existing directories required for the given path
        /// </summary>
        /// <param name="p_Path">Path for directories to create</param>
        private void CreateDirectories(string p_Path)
        {
            string v_RootDir = Path.GetDirectoryName(p_Path);
            Directory.CreateDirectory(v_RootDir);
        }
    }
}