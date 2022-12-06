using System;
using System.Runtime.InteropServices;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    internal class LibsodiumImpl : ILibsodiumImpl
    {
        internal const string Library = "libsodium";

        public int CryptoBoxMacBytes() =>
            crypto_box_macbytes();
        public int CryptoBoxNonceBytes() =>
            crypto_box_noncebytes();
        public int CryptoBoxSeal(byte[] c, byte[] m, ulong mlen, byte[] pk) =>
            crypto_box_seal(c, m, mlen, pk);
        public int CryptoBoxSealOpen(byte[] m, byte[] c, ulong clen, byte[] pk, byte[] sk) =>
            crypto_box_seal_open(m, c, clen, pk, sk);
        public int CryptoGenericHash(byte[] buffer, int bufferLength, byte[] message, long messageLength, byte[] key, int keyLength) =>
            crypto_generichash(buffer, bufferLength, message, messageLength, key, keyLength);
        public int CryptoGenericHashBlake2b(byte[] @out, nuint outlen, byte[] @in, ulong inlen, byte[] key, nuint keylen) =>
            crypto_generichash_blake2b(@out, outlen, @in, inlen, key, keylen);
        public int CryptoKxClientSessionKeys(byte[] rx, byte[] tx, byte[] client_pk, byte[] client_sk, byte[] server_pk) =>
            crypto_kx_client_session_keys(rx, tx, client_pk, client_sk, server_pk);
        public int CryptoKxServerSessionKeys(byte[] rx, byte[] tx, byte[] server_pk, byte[] server_sk, byte[] client_pk) =>
            crypto_kx_server_session_keys(rx, tx, server_pk, server_sk, client_pk);
        public int CryptoSecretBoxEasy(byte[] c, byte[] m, ulong mlen, byte[] n, byte[] k) =>
            crypto_secretbox_easy(c, m, mlen, n, k);
        public int CryptoSecretBoxOpenEasy(byte[] m, byte[] c, ulong clen, byte[] n, byte[] k) =>
            crypto_secretbox_open_easy(m, c, clen, n, k);
        public int CryptoSignEd25519(byte[] sm, ref ulong smlen_p, byte[] m, ulong mlen, byte[] sk) =>
            crypto_sign_ed25519(sm, ref smlen_p, m, mlen, sk);
        public int CryptoSignEd25519Detached(byte[] sig, ref ulong siglen_p, byte[] m, ulong mlen, byte[] sk) =>
            crypto_sign_ed25519_detached(sig, ref siglen_p, m, mlen, sk);
        public int CryptoSignEd25519KeyPair(byte[] pk, byte[] sk) =>
            crypto_sign_ed25519_keypair(pk, sk);
        public int CryptoSignEd25519Open(byte[] m, ref ulong mlen_p, byte[] sm, ulong smlen, byte[] pk) =>
            crypto_sign_ed25519_open(m, ref mlen_p, sm, smlen, pk);
        public int CryptoSignEd25519PkToCurve25519(byte[] curve25519_pk, byte[] ed25519_pk) =>
            crypto_sign_ed25519_pk_to_curve25519(curve25519_pk, ed25519_pk);
        public int CryptoSignEd25519SeedKeyPair(byte[] pk, byte[] sk, byte[] seed) =>
            crypto_sign_ed25519_seed_keypair(pk, sk, seed);
        public int CryptoSignEd25519SkToCurve25519(byte[] curve25519_sk, byte[] ed25519_sk) =>
            crypto_sign_ed25519_sk_to_curve25519(curve25519_sk, ed25519_sk);
        public int CryptoSignEd25519SkToPk(byte[] pk, byte[] sk) =>
            crypto_sign_ed25519_sk_to_pk(pk, sk);
        public int CryptoSignEd25519SkToSeed(byte[] seed, byte[] sk) =>
            crypto_sign_ed25519_sk_to_seed(seed, sk);
        public int CryptoSignEd25519VerifyDetached(byte[] sig, byte[] m, ulong mlen, byte[] pk) =>
            crypto_sign_ed25519_verify_detached(sig, m, mlen, pk);
        public int SodiumInit() =>
            sodium_init();
        public int SodiumLibraryVersionMajor() =>
            sodium_library_version_major();
        public int SodiumLibraryVersionMinor() =>
            sodium_library_version_minor();
        public int SodiumSetMisuseHandler(Action handler) =>
            sodium_set_misuse_handler(handler);
        public IntPtr SodiumVersionString() =>
            sodium_version_string();

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
            nuint outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            nuint keylen);

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
            long messageLength,
            byte[] key,
            int keyLength);
    }
}