using System;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    interface ILibsodiumWrapper
    {
        int sodium_init();

        int sodium_set_misuse_handler(Action handler);

        int sodium_library_version_major();

        int sodium_library_version_minor();

        IntPtr sodium_version_string();

        int crypto_sign_ed25519(
            byte[] sm,
            ref ulong smlen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        int crypto_sign_ed25519_detached(
            byte[] sig,
            ref ulong siglen_p,
            byte[] m,
            ulong mlen,
            byte[] sk);

        int crypto_sign_ed25519_keypair(
            byte[] pk,
            byte[] sk);

        int crypto_sign_ed25519_open(
            byte[] m,
            ref ulong mlen_p,
            byte[] sm,
            ulong smlen,
            byte[] pk);

        int crypto_sign_ed25519_pk_to_curve25519(
            byte[] curve25519_pk,
            byte[] ed25519_pk);

        int crypto_sign_ed25519_seed_keypair(
            byte[] pk,
            byte[] sk,
            byte[] seed);

        int crypto_sign_ed25519_sk_to_curve25519(
            byte[] curve25519_sk,
            byte[] ed25519_sk);

        int crypto_sign_ed25519_sk_to_pk(
            byte[] pk,
            byte[] sk);

        int crypto_sign_ed25519_sk_to_seed(
            byte[] seed,
            byte[] sk);

        int crypto_sign_ed25519_verify_detached(
            byte[] sig,
            byte[] m,
            ulong mlen,
            byte[] pk);

        int crypto_generichash_blake2b(
            byte[] @out,
            nuint outlen,
            byte[] @in,
            ulong inlen,
            byte[] key,
            nuint keylen);

        int crypto_secretbox_easy(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] n,
            byte[] k);

        int crypto_secretbox_open_easy(
           byte[] m,
           byte[] c,
           ulong clen,
           byte[] n,
           byte[] k);

        int crypto_box_seal(
            byte[] c,
            byte[] m,
            ulong mlen,
            byte[] pk);

        int crypto_box_seal_open(
            byte[] m,
            byte[] c,
            ulong clen,
            byte[] pk,
            byte[] sk);

        int crypto_kx_client_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] client_pk,
            byte[] client_sk,
            byte[] server_pk);

        int crypto_kx_server_session_keys(
            byte[] rx,
            byte[] tx,
            byte[] server_pk,
            byte[] server_sk,
            byte[] client_pk);

        int crypto_box_macbytes();

        int crypto_box_noncebytes();

        int crypto_generichash(
            byte[] buffer,
            int bufferLength,
            byte[] message,
            long messageLength,
            byte[] key,
            int keyLength);
    }
}