// ReSharper disable InconsistentNaming
namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    using System.Runtime.InteropServices;

    public static class SodiumLibrary
    {
#if IOS
        const string DllName = "__Internal";
#else
        private const string DllName = "libsodium";
#endif

        //crypto_kx_client_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_client_session_keys(byte[] rx, byte[] tx, byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk);
        
        // crypto_kx_server_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_server_session_keys(byte[] rx, byte[] tx, byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_macbytes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_noncebytes();
    }
}