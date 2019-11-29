using System;
using System.Runtime.InteropServices;
using System.Security;

namespace pillont.CommonTools.Core.Security
{
    public static class SecurePasswordHelper
    {
        public static void SetValue(this SecureString securePassword, string value)
        {
            securePassword.Clear();
            foreach (var c in value)
                securePassword.AppendChar(c);
        }

        // SOURCE :https://code.msdn.microsoft.com/windowsdesktop/Get-Password-from-df012a86
        public static string UnSecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
