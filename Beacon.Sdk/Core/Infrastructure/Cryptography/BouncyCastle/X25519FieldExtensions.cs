//using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.BouncyCastle
{
    internal class PointExt
    {
        internal int[] x = X25519Field.Create();
        internal int[] y = X25519Field.Create();
        internal int[] z = X25519Field.Create();
        internal int[] t = X25519Field.Create();
    }

    internal static class X25519FieldExtensions
    {
        //public static void FromBytes(int[] f, byte[] bytes)
        //{
            
        //}

        public static void Reduce(ulong[] f, ulong[] result)
        {
            const ulong mask = 0x7ffffffffffffUL;
            var t = new BigInteger[5];

            t[0] = f[0];
            t[1] = f[1];
            t[2] = f[2];
            t[3] = f[3];
            t[4] = f[4];

            t[1] += t[0] >> 51;
            t[0] &= mask;
            t[2] += t[1] >> 51;
            t[1] &= mask;
            t[3] += t[2] >> 51;
            t[2] &= mask;
            t[4] += t[3] >> 51;
            t[3] &= mask;
            t[0] += 19 * (t[4] >> 51);
            t[4] &= mask;

            t[1] += t[0] >> 51;
            t[0] &= mask;
            t[2] += t[1] >> 51;
            t[1] &= mask;
            t[3] += t[2] >> 51;
            t[2] &= mask;
            t[4] += t[3] >> 51;
            t[3] &= mask;
            t[0] += 19 * (t[4] >> 51);
            t[4] &= mask;

            /* now t is between 0 and 2^255-1, properly carried. */
            /* case 1: between 0 and 2^255-20. case 2: between 2^255-19 and 2^255-1. */

            t[0] += 19UL;

            t[1] += t[0] >> 51;
            t[0] &= mask;
            t[2] += t[1] >> 51;
            t[1] &= mask;
            t[3] += t[2] >> 51;
            t[2] &= mask;
            t[4] += t[3] >> 51;
            t[3] &= mask;
            t[0] += 19UL * (t[4] >> 51);
            t[4] &= mask;

            /* now between 19 and 2^255-1 in both cases, and offset by 19. */

            t[0] += 0x8000000000000 - 19UL;
            t[1] += 0x8000000000000 - 1UL;
            t[2] += 0x8000000000000 - 1UL;
            t[3] += 0x8000000000000 - 1UL;
            t[4] += 0x8000000000000 - 1UL;

            /* now between 2^255 and 2^256-20, and offset by 2^255. */

            t[1] += t[0] >> 51;
            t[0] &= mask;
            t[2] += t[1] >> 51;
            t[1] &= mask;
            t[3] += t[2] >> 51;
            t[2] &= mask;
            t[4] += t[3] >> 51;
            t[3] &= mask;
            t[4] &= mask;

            result[0] = (ulong)t[0];
            result[1] = (ulong)t[1];
            result[2] = (ulong)t[2];
            result[3] = (ulong)t[3];
            result[4] = (ulong)t[4];
        }

        public static byte[] ToBytes(int[] h)
        {
            ulong t0, t1, t2, t3;

            var hl = ToUlongs(h);

            var t = new ulong[5];

            Reduce(hl, t);

            t0 = t[0] | (t[1] << 51);
            t1 = (t[1] >> 13) | (t[2] << 38);
            t2 = (t[2] >> 26) | (t[3] << 25);
            t3 = (t[3] >> 39) | (t[4] << 12);

            var s = new byte[32];

            STORE64_LE(t0, s, 0);
            STORE64_LE(t1, s, 8);
            STORE64_LE(t2, s, 16);
            STORE64_LE(t3, s, 24);

            return s;
        }

        public static ulong[] ToUlongs(int[] f)
        {
            var h = new ulong[X25519Field.Size / 2];

            h[0] = (ulong)f[0] | (ulong)(f[1] << 32);
            h[1] = (ulong)f[2] | (ulong)(f[3] << 32);
            h[2] = (ulong)f[4] | (ulong)(f[5] << 32);
            h[3] = (ulong)f[6] | (ulong)(f[7] << 32);
            h[4] = (ulong)f[8] | (ulong)(f[9] << 32);

            return h;
        }

        public static int[] ToInts(ulong[] f)
        {
            var h = new int[X25519Field.Size];

            h[0] = (int)f[0];
            h[1] = (int)(f[0] >> 32);
            h[2] = (int)f[1];
            h[3] = (int)(f[1] >> 32);
            h[4] = (int)f[2];
            h[5] = (int)(f[2] >> 32);
            h[6] = (int)f[3];
            h[7] = (int)(f[3] >> 32);
            h[8] = (int)f[4];
            h[9] = (int)(f[4] >> 32);

            return h;
        }

        public static void STORE64_LE(ulong w, byte[] result, int resultOffset)
        {
            result[resultOffset + 0] = (byte)w; w >>= 8;
            result[resultOffset + 1] = (byte)w; w >>= 8;
            result[resultOffset + 2] = (byte)w; w >>= 8;
            result[resultOffset + 3] = (byte)w; w >>= 8;
            result[resultOffset + 4] = (byte)w; w >>= 8;
            result[resultOffset + 5] = (byte)w; w >>= 8;
            result[resultOffset + 6] = (byte)w; w >>= 8;
            result[resultOffset + 7] = (byte)w; 
        }
    }
}