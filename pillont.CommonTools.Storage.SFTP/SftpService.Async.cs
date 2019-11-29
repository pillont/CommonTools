using pillont.CommonTools.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pillont.CommonTools.Storage.SFTP
{
    /// <summary>
    /// async func to manage files by SFTP
    /// </summary>
    public partial class SftpService : IAsyncStorageProvider
    {
        public async Task DeleteAsync(string p_Path)
        {
            await Task.Run(() => Delete(p_Path));
        }

        public async Task<bool> FileExistsAsync(string p_Path)
        {
            return await Task.Run(() =>
                                    FileExists(p_Path));
        }

        public async Task<byte[]> GetByteAsync(string p_Path)
        {
            return await Task.Run(() =>
                                    GetByte(p_Path));
        }

        public async Task<string> GetStringAsync(string p_Path)
        {
            return await Task.Run(() =>
                                    GetString(p_Path));
        }

        public async Task<IEnumerable<string>> SearchAsync(string p_Path, string p_Pattern, bool p_Recurse = false, Func<IEnumerable<string>, IEnumerable<string>> p_Predicate = null)
        {
            return await Task.Run(() =>
                                    Search(p_Path, p_Pattern, p_Recurse, p_Predicate));
        }

        public async Task StoreAsync(string p_Path, string p_Content)
        {
            await Task.Run(() =>
                            Store(p_Path, p_Content));
        }

        public async Task StoreAsync(string p_Path, byte[] p_ByteContent)
        {
            await Task.Run(() =>
                            Store(p_Path, p_ByteContent));
        }
    }
}