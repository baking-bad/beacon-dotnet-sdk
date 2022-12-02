// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public enum SodiumLibraryType
    {
        /// <summary>
        /// Default dynamic libraries
        /// </summary>
        /// <remarks>
        /// Library name: libsodium.dll for Windows, libsodium.so for Linux/Android, libsodium.dylib for macOS
        /// </remarks>
        Dynamic,
        /// <summary>
        /// Dynamic library as part of the iOS framework
        /// </summary>
        /// <remarks>
        /// Library name: libsodium.framework/libsodium
        /// </remarks>
        DynamicFramework,
        /// <summary>
        /// Static library (default for iOS)
        /// </summary>
        /// <remarks>
        /// Library name: libsodium.a, actual used name: "__Internal"
        /// </remarks>
        StaticInternal
    }

    public static class Sodium
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

        static ILibsodiumImpl _impl = new LibsodiumImpl();

        public static void SetLibraryType(SodiumLibraryType type)
        {
            _impl = type switch
            {
                SodiumLibraryType.Dynamic => new LibsodiumImpl(),
                SodiumLibraryType.DynamicFramework => new FrameworkLibsodiumImpl(),
                SodiumLibraryType.StaticInternal => new InternalLibsodiumImpl(),
                _ => new LibsodiumImpl(),
            };
        }

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
                if (SodiumLibraryVersionMajor() != SODIUM_LIBRARY_VERSION_MAJOR ||
                    SodiumLibraryVersionMinor() != SODIUM_LIBRARY_VERSION_MINOR)
                {
                    string? version = Marshal.PtrToStringAnsi(SodiumVersionString());
                    throw (version != null && version != SODIUM_VERSION_STRING)
                        ? new NotSupportedException($"An error occurred while initializing cryptographic primitives. (Expected libsodium {SODIUM_VERSION_STRING} but found {version}.)")
                        : new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                if (SodiumSetMisuseHandler(s_misuseHandler) != 0)
                {
                    throw new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                // sodium_init() returns 0 on success, -1 on failure, and 1 if the library had already been initialized.
                if (SodiumInit() < 0)
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

        internal static int SodiumInit() =>
            _impl.SodiumInit();

        internal static int SodiumSetMisuseHandler(Action handler) =>
            _impl.SodiumSetMisuseHandler(handler);

        internal static int SodiumLibraryVersionMajor() =>
            _impl.SodiumLibraryVersionMajor();

        internal static int SodiumLibraryVersionMinor() =>
            _impl.SodiumLibraryVersionMinor();

        internal static IntPtr SodiumVersionString() =>
            _impl.SodiumVersionString();

        internal static int CryptoSignEd25519(
            byte[] sm,
            ref ulong smlen_p,
            byte[] m,
            ulong mlen,
            byte[] sk) => _impl.CryptoSignEd25519(sm, ref smlen_p, m, mlen, sk);

        internal static int CryptoSignEd25519Detached(
            byte[] sig,
            ref ulong siglen_p,
            byte[] m,
            ulong mlen,
            byte[] sk) => _impl.CryptoSignEd25519Detached(sig, ref siglen_p, m, mlen, sk);

        internal static int CryptoSignEd25519KeyPair(
            byte[] pk,
            byte[] sk) => _impl.CryptoSignEd25519KeyPair(pk, sk);

        internal static int CryptoSignEd25519Open(
            byte[] m,
            ref ulong mlen_p,
            byte[] sm,
            ulong smlen,
            byte[] pk) => _impl.CryptoSignEd25519Open(m, ref mlen_p, sm, smlen, pk);

        internal static int CryptoSignEd25519PkToCurve25519(
            byte[] curve25519_pk,
            byte[] ed25519_pk) => _impl.CryptoSignEd25519PkToCurve25519(curve25519_pk, ed25519_pk);

        internal static int CryptoSignEd25519SeedKeyPair(
            byte[] pk,
            byte[] sk,
            byte[] seed) => _impl.CryptoSignEd25519SeedKeyPair(pk, sk, seed);

        internal static int CryptoSignEd25519SkToCurve25519(
            byte[] curve25519_sk,
            byte[] ed25519_sk) => _impl.CryptoSignEd25519SkToCurve25519(curve25519_sk, ed25519_sk);

        internal static int CryptoSignEd25519SkToPk(
            byte[] pk,
            byte[] sk) => _impl.CryptoSignEd25519SkToPk(pk, sk);

        internal static int CryptoSignEd25519SkToSeed(
            byte[] seed,
            byte[] sk) => _impl.CryptoSignEd25519SkToSeed(seed, sk);

        internal static int CryptoSignEd25519VerifyDetached(
            byte[] sig,
            byte[] m,
            ulong mlen,
            byte[] pk) => _impl.CryptoSignEd25519VerifyDetached(sig, m, mlen, pk);

        internal static int CryptoGenericHashBlake2b(
            byte[] @out,
            nuint outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            nuint keylen) => _impl.CryptoGenericHashBlake2b(@out, outlen, @in, inlen, key, keylen);

        internal static int CryptoSecretBoxEasy(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] n,
            byte[] k) => _impl.CryptoSecretBoxEasy(c, m, mlen, n, k);

        internal static int CryptoSecretBoxOpenEasy(
           byte[] m,
           byte[] c,
           ulong clen,
           byte[] n,
           byte[] k) => _impl.CryptoSecretBoxOpenEasy(m, c, clen, n, k);

        internal static int CryptoBoxSeal(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] pk) => _impl.CryptoBoxSeal(c, m, mlen, pk);

        internal static int CryptoBoxSealOpen(
            byte[] m,
            byte[] c,
            ulong clen,
            byte[] pk,
            byte[] sk) => _impl.CryptoBoxSealOpen(m, c, clen, pk, sk);

        internal static int CryptoKxClientSessionKeys(
            byte[] rx,
            byte[] tx,
            byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk) => _impl.CryptoKxClientSessionKeys(rx, tx, client_pk, client_sk, server_pk);

        internal static int CryptoKxServerSessionKeys(
            byte[] rx,
            byte[] tx,
            byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk) => _impl.CryptoKxServerSessionKeys(rx, tx, server_pk, server_sk, client_pk);

        internal static int CryptoBoxMacBytes() =>
            _impl.CryptoBoxMacBytes();

        internal static int CryptoBoxNonceBytes() =>
            _impl.CryptoBoxNonceBytes();

        internal static int CryptoGenericHash(
            byte[] buffer,
            int bufferLength,
            byte[] message,
            long messageLength,
            byte[] key,
            int keyLength) => _impl.CryptoGenericHash(buffer, bufferLength, message, messageLength, key, keyLength);
    }
}