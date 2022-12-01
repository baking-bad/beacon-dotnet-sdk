using System;
using System.Runtime.InteropServices;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    internal class InternalLibsodiumWrapper : ILibsodiumWrapper
    {
        internal const string DllName = "__Internal";

        public int crypto_box_macbytes()
        {
            return _crypto_box_macbytes();
        }

        public int crypto_box_noncebytes()
        {
            return _crypto_box_noncebytes();
        }

        public int crypto_box_seal(byte[] c, byte[] m, ulong mlen, byte[] pk)
        {
            return _crypto_box_seal(c, m, mlen, pk);
        }

        public int crypto_box_seal_open(byte[] m, byte[] c, ulong clen, byte[] pk, byte[] sk)
        {
            return _crypto_box_seal_open(m, c, clen, pk, sk);
        }

        public int crypto_generichash(byte[] buffer, int bufferLength, byte[] message, long messageLength, byte[] key, int keyLength)
        {
            return _crypto_generichash(buffer, bufferLength, message, messageLength, key, keyLength);
        }

        public int crypto_generichash_blake2b(byte[] @out, nuint outlen, byte[] @in, ulong inlen, byte[] key, nuint keylen)
        {
            return _crypto_generichash_blake2b(@out, outlen, @in, inlen, key, keylen);
        }

        public int crypto_kx_client_session_keys(byte[] rx, byte[] tx, byte[] client_pk, byte[] client_sk, byte[] server_pk)
        {
            return _crypto_kx_client_session_keys(rx, tx, client_pk, client_sk, server_pk);
        }

        public int crypto_kx_server_session_keys(byte[] rx, byte[] tx, byte[] server_pk, byte[] server_sk, byte[] client_pk)
        {
            return _crypto_kx_server_session_keys(rx, tx, server_pk, server_sk, client_pk);
        }

        public int crypto_secretbox_easy(byte[] c, byte[] m, ulong mlen, byte[] n, byte[] k)
        {
            return _crypto_secretbox_easy(c, m, mlen, n, k);
        }

        public int crypto_secretbox_open_easy(byte[] m, byte[] c, ulong clen, byte[] n, byte[] k)
        {
            return _crypto_secretbox_open_easy(m, c, clen, n, k);
        }

        public int crypto_sign_ed25519(byte[] sm, ref ulong smlen_p, byte[] m, ulong mlen, byte[] sk)
        {
            return _crypto_sign_ed25519(sm, ref smlen_p, m, mlen, sk);
        }

        public int crypto_sign_ed25519_detached(byte[] sig, ref ulong siglen_p, byte[] m, ulong mlen, byte[] sk)
        {
            return _crypto_sign_ed25519_detached(sig, ref siglen_p, m, mlen, sig);
        }

        public int crypto_sign_ed25519_keypair(byte[] pk, byte[] sk)
        {
            return _crypto_sign_ed25519_keypair(pk, sk);
        }

        public int crypto_sign_ed25519_open(byte[] m, ref ulong mlen_p, byte[] sm, ulong smlen, byte[] pk)
        {
            return _crypto_sign_ed25519_open(m, ref mlen_p, sm, smlen, pk);
        }

        public int crypto_sign_ed25519_pk_to_curve25519(byte[] curve25519_pk, byte[] ed25519_pk)
        {
            return _crypto_sign_ed25519_pk_to_curve25519(curve25519_pk, ed25519_pk);
        }

        public int crypto_sign_ed25519_seed_keypair(byte[] pk, byte[] sk, byte[] seed)
        {
            return _crypto_sign_ed25519_seed_keypair(pk, sk, seed);
        }

        public int crypto_sign_ed25519_sk_to_curve25519(byte[] curve25519_sk, byte[] ed25519_sk)
        {
            return _crypto_sign_ed25519_sk_to_curve25519(curve25519_sk, ed25519_sk);
        }

        public int crypto_sign_ed25519_sk_to_pk(byte[] pk, byte[] sk)
        {
            return _crypto_sign_ed25519_sk_to_pk(pk, sk);
        }

        public int crypto_sign_ed25519_sk_to_seed(byte[] seed, byte[] sk)
        {
            return _crypto_sign_ed25519_sk_to_seed(seed, sk);
        }

        public int crypto_sign_ed25519_verify_detached(byte[] sig, byte[] m, ulong mlen, byte[] pk)
        {
            return _crypto_sign_ed25519_verify_detached(sig, m, mlen, pk);
        }

        public int sodium_init()
        {
            return _sodium_init();
        }

        public int sodium_library_version_major()
        {
            return _sodium_library_version_major();
        }

        public int sodium_library_version_minor()
        {
            return _sodium_library_version_minor();
        }

        public int sodium_set_misuse_handler(Action handler)
        {
            return _sodium_set_misuse_handler(handler);
        }

        public IntPtr sodium_version_string()
        {
            return _sodium_version_string();
        }

        #region Initialization

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sodium_init")]
        internal static extern int _sodium_init();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sodium_set_misuse_handler")]
        internal static extern int _sodium_set_misuse_handler(Action handler);

        #endregion

        #region Version

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sodium_library_version_major")]
        internal static extern int _sodium_library_version_major();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sodium_library_version_minor")]
        internal static extern int _sodium_library_version_minor();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sodium_version_string")]
        internal static extern IntPtr _sodium_version_string();

        #endregion

        #region Ed25519

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519")]
        internal static extern int _crypto_sign_ed25519(
            byte[] sm,
            ref ulong smlen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_detached")]
        internal static extern int _crypto_sign_ed25519_detached(
            byte[] sig,
            ref ulong siglen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_keypair")]
        internal static extern int _crypto_sign_ed25519_keypair(
            byte[] pk,
            byte[] sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_open")]
        internal static extern int _crypto_sign_ed25519_open(
            byte[] m,
            ref ulong mlen_p,
            byte[] sm,
            ulong smlen,
            byte[] pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_pk_to_curve25519")]
        internal static extern int _crypto_sign_ed25519_pk_to_curve25519(
            byte[] curve25519_pk,
            byte[] ed25519_pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_seed_keypair")]
        internal static extern int _crypto_sign_ed25519_seed_keypair(
            byte[] pk,
            byte[] sk,
            byte[] seed);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_sk_to_curve25519")]
        internal static extern int _crypto_sign_ed25519_sk_to_curve25519(
            byte[] curve25519_sk,
            byte[] ed25519_sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_sk_to_pk")]
        internal static extern int _crypto_sign_ed25519_sk_to_pk(
            byte[] pk,
            byte[] sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_sk_to_seed")]
        internal static extern int _crypto_sign_ed25519_sk_to_seed(
            byte[] seed,
            byte[] sk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_sign_ed25519_verify_detached")]
        internal static extern int _crypto_sign_ed25519_verify_detached(
            byte[] sig,
            byte[] m,
            ulong mlen,
            byte[] pk);

        #endregion

        #region GenericHash

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_generichash_blake2b")]
        internal static extern int _crypto_generichash_blake2b(
            byte[] @out,
            nuint outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            nuint keylen);

        #endregion

        #region SecretBox


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_easy")]
        internal static extern int _crypto_secretbox_easy(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] n,
            byte[] k);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_open_easy")]
        internal static extern int _crypto_secretbox_open_easy(
           byte[] m,
           byte[] c,
           ulong clen,
           byte[] n,
           byte[] k);

        #endregion

        #region SealedPublicKeyBox

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_seal")]
        internal static extern int _crypto_box_seal(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_seal_open")]
        internal static extern int _crypto_box_seal_open(
            byte[] m,
            byte[] c,
            ulong clen,
            byte[] pk,
            byte[] sk);

        #endregion

        //crypto_kx_client_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_kx_client_session_keys")]
        internal static extern int _crypto_kx_client_session_keys(byte[] rx, byte[] tx, byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk);

        // crypto_kx_server_session_keys
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_kx_server_session_keys")]
        internal static extern int _crypto_kx_server_session_keys(byte[] rx, byte[] tx, byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_macbytes")]
        internal static extern int _crypto_box_macbytes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_noncebytes")]
        internal static extern int _crypto_box_noncebytes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_generichash")]
        internal static extern int _crypto_generichash(byte[] buffer, int bufferLength, byte[] message,
            long messageLength, byte[] key,
            int keyLength);
    }
}