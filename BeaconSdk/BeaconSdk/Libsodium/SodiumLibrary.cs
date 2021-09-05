// ReSharper disable InconsistentNaming
namespace BeaconSdk.Libsodium
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Sodium;

    public static class SodiumLibrary 
    {
#if IOS
        const string DllName = "__Internal";
#else
        const string DllName = "libsodium";
#endif

        //sodium_init
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Int32 sodium_init();
        
        //crypto_kx_client_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_client_session_keys(byte[] rx, byte[] tx, byte[] client_pk, byte[] client_sk, 
            byte[] server_pk);
        
        // //crypto_kx_client_session_keys
        // [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        // internal static extern int crypto_kx_client_session_keys(Span<byte> rx, Span<byte> tx, 
        //     Span<byte> client_pk, Span<byte> client_sk, Span<byte> server_pk);

        // int crypto_kx_client_session_keys(unsigned char rx[crypto_kx_SESSIONKEYBYTES],
        //     unsigned char tx[crypto_kx_SESSIONKEYBYTES],
        // const unsigned char client_pk[crypto_kx_PUBLICKEYBYTES],
        // const unsigned char client_sk[crypto_kx_SECRETKEYBYTES],
        // const unsigned char server_pk[crypto_kx_PUBLICKEYBYTES]);
        
        // crypto_kx_server_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_server_session_keys(byte[] rx, byte[] tx, byte[] server_pk, byte[] server_sk, 
            byte[] client_pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_macbytes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_noncebytes();

    }


}