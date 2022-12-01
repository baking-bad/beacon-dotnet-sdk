// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    using System;

    public enum SodiumLibraryType
    {
        Default,
        Static
    }

    public static class SodiumLibrary
    {
        static ILibsodiumWrapper _impl = new LibsodiumWrapper();

        public static void SetLibraryType(SodiumLibraryType type)
        {
            _impl = type switch
            {
                SodiumLibraryType.Default => new LibsodiumWrapper(),
                SodiumLibraryType.Static => new InternalLibsodiumWrapper(),
                _ => new LibsodiumWrapper(),
            };
        }

        #region Initialization

        internal static int sodium_init() =>
            _impl.sodium_init();

        internal static int sodium_set_misuse_handler(Action handler) =>
            _impl.sodium_set_misuse_handler(handler);

        #endregion

        #region Version

        internal const int SODIUM_LIBRARY_VERSION_MAJOR = 10;
        internal const int SODIUM_LIBRARY_VERSION_MINOR = 3;
        internal const string SODIUM_VERSION_STRING = "1.0.18";

        internal static int sodium_library_version_major() =>
            _impl.sodium_library_version_major();

        internal static int sodium_library_version_minor() =>
            _impl.sodium_library_version_minor();

        internal static IntPtr sodium_version_string() =>
            _impl.sodium_version_string();

        #endregion

        #region Ed25519

        internal const int crypto_sign_ed25519_BYTES = 64;
        internal const int crypto_sign_ed25519_PUBLICKEYBYTES = 32;
        internal const int crypto_sign_ed25519_SECRETKEYBYTES = (32 + 32);
        internal const int crypto_sign_ed25519_SEEDBYTES = 32;

        internal static int crypto_sign_ed25519(
            byte[] sm,
            ref ulong smlen_p,
            byte[] m,
            ulong mlen,
            byte[] sk) => _impl.crypto_sign_ed25519(sm, ref smlen_p, m, mlen, sk);

        internal static int crypto_sign_ed25519_detached(
            byte[] sig,
            ref ulong siglen_p,
            byte[] m,
            ulong mlen,
            byte[] sk) => _impl.crypto_sign_ed25519_detached(sig, ref siglen_p, m, mlen, sk);

        internal static int crypto_sign_ed25519_keypair(
            byte[] pk,
            byte[] sk) => _impl.crypto_sign_ed25519_keypair(pk, sk);

        internal static int crypto_sign_ed25519_open(
            byte[] m,
            ref ulong mlen_p,
            byte[] sm,
            ulong smlen,
            byte[] pk) => _impl.crypto_sign_ed25519_open(m, ref mlen_p, sm, smlen, pk);

        internal static int crypto_sign_ed25519_pk_to_curve25519(
            byte[] curve25519_pk,
            byte[] ed25519_pk) => _impl.crypto_sign_ed25519_pk_to_curve25519(curve25519_pk, ed25519_pk);

        internal static int crypto_sign_ed25519_seed_keypair(
            byte[] pk,
            byte[] sk,
            byte[] seed) => _impl.crypto_sign_ed25519_seed_keypair(pk, sk, seed);

        internal static int crypto_sign_ed25519_sk_to_curve25519(
            byte[] curve25519_sk,
            byte[] ed25519_sk) => _impl.crypto_sign_ed25519_sk_to_curve25519(curve25519_sk, ed25519_sk);

        internal static int crypto_sign_ed25519_sk_to_pk(
            byte[] pk,
            byte[] sk) => _impl.crypto_sign_ed25519_sk_to_pk(pk, sk);

        internal static int crypto_sign_ed25519_sk_to_seed(
            byte[] seed,
            byte[] sk) => _impl.crypto_sign_ed25519_sk_to_seed(seed, sk);

        internal static int crypto_sign_ed25519_verify_detached(
            byte[] sig,
            byte[] m,
            ulong mlen,
            byte[] pk) => _impl.crypto_sign_ed25519_verify_detached(sig, m, mlen, pk);

        #endregion

        #region X25519

        internal const int crypto_scalarmult_curve25519_BYTES = 32;
        internal const int crypto_scalarmult_curve25519_SCALARBYTES = 32;

        #endregion

        #region GenericHash

        internal const int crypto_generichash_blake2b_BYTES_MAX = 64;
        internal const int crypto_generichash_blake2b_BYTES_MIN = 16;
        internal const int crypto_generichash_blake2b_KEYBYTES_MAX = 64;
        internal const int crypto_generichash_blake2b_KEYBYTES_MIN = 16;

        internal static int crypto_generichash_blake2b(
            byte[] @out,
            nuint outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            nuint keylen) => _impl.crypto_generichash_blake2b(@out, outlen, @in, inlen, key, keylen);

        #endregion

        #region SecretBox

        internal const int crypto_secretbox_xsalsa20poly1305_KEYBYTES = 32;
        internal const int crypto_secretbox_xsalsa20poly1305_MACBYTES = 16;
        internal const int crypto_secretbox_xsalsa20poly1305_NONCEBYTES = 24;

        internal static int crypto_secretbox_easy(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] n,
            byte[] k) => _impl.crypto_secretbox_easy(c, m, mlen, n, k);

        internal static int crypto_secretbox_open_easy(
           byte[] m,
           byte[] c,
           ulong clen,
           byte[] n,
           byte[] k) => _impl.crypto_secretbox_open_easy(m, c, clen, n, k);

        #endregion

        #region SealedPublicKeyBox

        internal const int crypto_box_curve25519xsalsa20poly1305_MACBYTES = 16;
        internal const int crypto_box_curve25519xsalsa20poly1305_PUBLICKEYBYTES = 32;
        internal const int crypto_box_curve25519xsalsa20poly1305_SECRETKEYBYTES = 32;

        internal static int crypto_box_seal(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] pk) => _impl.crypto_box_seal(c, m, mlen, pk);

        internal static int crypto_box_seal_open(
            byte[] m,
            byte[] c,
            ulong clen,
            byte[] pk,
            byte[] sk) => _impl.crypto_box_seal_open(m, c, clen, pk, sk);

        #endregion

        //crypto_kx_client_session_keys
        internal static int crypto_kx_client_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk) => _impl.crypto_kx_client_session_keys(rx, tx, client_pk, client_sk, server_pk);

        // crypto_kx_server_session_keys
        internal static int crypto_kx_server_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk) => _impl.crypto_kx_server_session_keys(rx, tx, server_pk, server_sk, client_pk);

        internal static int crypto_box_macbytes() =>
            _impl.crypto_box_macbytes();

        internal static int crypto_box_noncebytes() =>
            _impl.crypto_box_noncebytes();

        internal static int crypto_generichash(
            byte[] buffer,
            int bufferLength,
            byte[] message,
            long messageLength,
            byte[] key,
            int keyLength) => _impl.crypto_generichash(buffer, bufferLength, message, messageLength, key, keyLength);
    }
}