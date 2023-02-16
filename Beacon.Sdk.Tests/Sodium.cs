using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Beacon.Sdk.Tests
{
    internal static class Sodium
    {
        internal const int SODIUM_LIBRARY_VERSION_MAJOR = 10;
        internal const int SODIUM_LIBRARY_VERSION_MINOR = 3;
        internal const string SODIUM_VERSION_STRING = "1.0.18";
        internal const int crypto_sign_ed25519_BYTES = 64;
        internal const int crypto_sign_ed25519_PUBLICKEYBYTES = 32;
        internal const int crypto_sign_ed25519_SECRETKEYBYTES = 32 + 32;
        internal const int crypto_sign_ed25519_SEEDBYTES = 32;
        internal const int crypto_scalarmult_curve25519_BYTES = 32;
        internal const int crypto_scalarmult_curve25519_SCALARBYTES = 32;
        internal const int crypto_generichash_blake2b_BYTES_MAX = 64;
        internal const int crypto_generichash_blake2b_BYTES_MIN = 16;
        internal const int crypto_generichash_blake2b_KEYBYTES_MAX = 64;
        internal const int crypto_generichash_blake2b_KEYBYTES_MIN = 16;
        internal const int crypto_secretbox_xsalsa20poly1305_KEYBYTES = 32;
        internal const int crypto_secretbox_xsalsa20poly1305_MACBYTES = 16;
        internal const int crypto_secretbox_xsalsa20poly1305_NONCEBYTES = 24;
        internal const int crypto_box_curve25519xsalsa20poly1305_MACBYTES = 16;
        internal const int crypto_box_curve25519xsalsa20poly1305_PUBLICKEYBYTES = 32;
        internal const int crypto_box_curve25519xsalsa20poly1305_SECRETKEYBYTES = 32;
        internal const string Library = "libsodium";

        private static readonly Action s_misuseHandler = new(InternalError);

        private static int s_initialized;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Initialize()
        {
            if (s_initialized == 0)
            {
                InitializeCore();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCore()
        {
            try
            {
                if (sodium_library_version_major() != SODIUM_LIBRARY_VERSION_MAJOR ||
                    sodium_library_version_minor() != SODIUM_LIBRARY_VERSION_MINOR)
                {
                    string version = Marshal.PtrToStringAnsi(sodium_version_string());

                    throw (version != null && version != SODIUM_VERSION_STRING)
                        ? new NotSupportedException($"An error occurred while initializing cryptographic primitives. (Expected libsodium {SODIUM_VERSION_STRING} but found {version}.)")
                        : new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                if (sodium_set_misuse_handler(s_misuseHandler) != 0)
                {
                    throw new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                // sodium_init() returns 0 on success, -1 on failure, and 1 if the library had already been initialized.
                if (sodium_init() < 0)
                {
                    throw new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }
            }
            catch (DllNotFoundException e)
            {
                throw new PlatformNotSupportedException("Could not initialize platform-specific components. libsodium-core may not be supported on this platform. See https://github.com/ektrah/libsodium-core/blob/master/INSTALL.md for more information.", e);
            }
            catch (BadImageFormatException e)
            {
                throw new PlatformNotSupportedException("Could not initialize platform-specific components. libsodium-core may not be supported on this platform. See https://github.com/ektrah/libsodium-core/blob/master/INSTALL.md for more information.", e);
            }

            Interlocked.Exchange(ref s_initialized, 1);
        }

        private static void InternalError()
        {
            throw new NotSupportedException("An internal error occurred.");
        }

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_init();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_set_misuse_handler(Action handler);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_library_version_major();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_library_version_minor();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sodium_version_string();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519(
            byte[] sm,
            ref ulong smlen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_detached(
            byte[] sig,
            ref ulong siglen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_keypair(
            byte[] pk,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_open(
            byte[] m,
            ref ulong mlen_p,
            byte[] sm,
            ulong smlen,
            byte[] pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_pk_to_curve25519(
            byte[] curve25519_pk,
            byte[] ed25519_pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_seed_keypair(
            byte[] pk,
            byte[] sk,
            byte[] seed);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_sk_to_curve25519(
            byte[] curve25519_sk,
            byte[] ed25519_sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_sk_to_pk(
            byte[] pk,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_sk_to_seed(
            byte[] seed,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_sign_ed25519_verify_detached(
            byte[] sig,
            byte[] m,
            ulong mlen,
            byte[] pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_generichash_blake2b(
            byte[] @out,
            int outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            int keylen);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_secretbox_easy(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] n,
            byte[] k);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_secretbox_open_easy(
           byte[] m,
           byte[] c,
           ulong clen,
           byte[] n,
           byte[] k);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_seal(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_seal_open(
            byte[] m,
            byte[] c,
            ulong clen,
            byte[] pk,
            byte[] sk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_client_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_kx_server_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_macbytes();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_box_noncebytes();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_generichash(
            byte[] buffer,
            int bufferLength,
            byte[] message,
            ulong messageLength,
            byte[] key,
            int keyLength);
    }
}