using pillont.CommonTools.Core;
using pillont.CommonTools.Storage.Abstractions;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pillont.CommonTools.Storage.SFTP
{
    /// <summary>
    /// service to manage files by SFTP
    /// </summary>
    public partial class SftpService : IStorageProvider, IDisposable
    {
        private const string PROTOCOL_NAME = "sftp";

        /// <summary>
        /// inform if can override during creation if file is existing
        /// </summary>
        public bool CanOverride { get; set; }

        /// <summary>
        /// if use single connection : global client
        /// </summary>
        public SftpClient Client { get; }

        /// <summary>
        ///  info to open connection
        /// </summary>
        public ConnectionInfo ConnectionInfo { get; }

        public SftpService(string host, string username, string password, bool useConnectionForEachRequest)
        {
            ConnectionInfo = new ConnectionInfo(host,
                                                    PROTOCOL_NAME,
                                                    new PasswordAuthenticationMethod(username, password));

            if (!useConnectionForEachRequest)
            {
                Client = new SftpClient(ConnectionInfo);
            }
        }

        public void Delete(string p_Path)
        {
            ApplyRequest(sftp =>
                        sftp.DeleteFile(p_Path));
        }

        public bool FileExists(string p_Path)
        {
            return ApplyRequest(sftp =>
                        sftp.Exists(p_Path));
        }

        public byte[] GetByte(string p_Path)
        {
            return ApplyRequest(sftp =>
            {
                using (var stream = new MemoryStream())
                {
                    sftp.DownloadFile(p_Path, stream);
                    return stream.ToArray();
                }
            });
        }

        public string GetString(string p_Path)
        {
            return ApplyRequest(sftp =>
            {
                using (var stream = new MemoryStream())
                {
                    sftp.DownloadFile(p_Path, stream);
                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                }
            });
        }

        public IEnumerable<string> Search(string path, string pattern, bool isRecursive = false, Func<IEnumerable<string>, IEnumerable<string>> filterAction = null)
        {
            return ApplyRequest(sftp =>
            {
                var allResults = sftp.ListDirectory(path)
                                         .ToList();

                var names = allResults
                                    .Select(file => file.Name)
                                    .Where(name => name.Contains(pattern));

                // NOTE : apply predicate on the result
                names = filterAction?.Invoke(names)
                            ?? names;

                if (!isRecursive)
                    return names;

                // NOTE : 'predicate' not usefull here
                //        is applied on previous treatment
                //        so applied in recurcive call
                return allResults
                                 // NOTE : AsParallel to apply recurcive Search in parallel
                                 .AsParallel()

                                 // NOTE : recurcive browse in sub directory
                                 .Where(f => f.IsDirectory)
                                 .SelectMany(dir =>
                                     Search(path + dir.Name, pattern, isRecursive, filterAction))

                                 // NOTE : AsParallel to keep all treatment in Plinq
                                 // WARNING : CS0618  'ParallelEnumerable.Concat<TSource>(ParallelQuery<TSource>, IEnumerable<TSource>)' est obsolète: 'The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.'
                                 .Concat(names.AsParallel());
            });
        }

        public void Store(string p_Path, string p_Content)
        {
            Store(p_Path, p_Content.GetBytes());
        }

        public void Store(string p_Path, byte[] p_ByteContent)
        {
            using (var stream = new MemoryStream(p_ByteContent))
            {
                ApplyRequest(sftp =>
                            sftp.UploadFile(stream, p_Path, CanOverride));
            }
        }

        /// <see cref="ApplyRequest(Action{SftpClient})"/>
        private T ApplyRequest<T>(Func<SftpClient, T> action)
        {
            var result = default(T);
            ApplyRequest(c => result = action(c));
            return result;
        }

        /// <summary>
        /// Apply request with correct connection :
        /// if use unique connection : select global client to apply action
        /// else : open new connection, apply action and close the connection
        /// </summary>
        private void ApplyRequest(Action<SftpClient> action)
        {
            if (Client != null)
            {
                if (!Client.IsConnected)
                    Client.Connect();

                action.Invoke(Client);
            }

            using (var sftp = new SftpClient(ConnectionInfo))
            {
                sftp.Connect();
                action.Invoke(sftp);
                sftp.Disconnect();
            }
        }

        #region dispose implementation

        ///  SOURCE https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        // Flag: Has Dispose already been called?
        private bool _disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client?.Disconnect();
                Client?.Dispose();
            }

            _disposed = true;
        }

        #endregion dispose implementation
    }
}