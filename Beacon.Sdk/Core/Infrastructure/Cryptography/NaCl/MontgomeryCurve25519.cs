using System;


namespace Beacon.Sdk.Core.Infrastructure.Cryptography.NaCl
{
    using Internal.Ed25519Ref10;

    public static class MontgomeryCurve25519
    {
        public static void EdwardsToMontgomery(ArraySegment<byte> montgomery, ArraySegment<byte> edwards)
        {
            FieldElement edwardsY, edwardsZ, montgomeryX;
            FieldOperations.fe_frombytes(out edwardsY, edwards.Array, edwards.Offset);
            FieldOperations.fe_1(out edwardsZ);
            EdwardsToMontgomeryX(out montgomeryX, ref edwardsY, ref edwardsZ);
            FieldOperations.fe_tobytes(montgomery.Array, montgomery.Offset, ref montgomeryX);
            montgomery.Array[montgomery.Offset + 31] |= (byte)(edwards.Array[edwards.Offset + 31] & 0x80); // copy sign
        }

        private static void EdwardsToMontgomeryX(out FieldElement montgomeryX, ref FieldElement edwardsY,
            ref FieldElement edwardsZ)
        {
            // montgomeryX = (edwardsZ + edwardsY) / (edwardsZ - edwardsY)
            FieldElement tempX, tempZ;
            FieldOperations.fe_add(out tempX, ref edwardsZ, ref edwardsY);
            FieldOperations.fe_sub(out tempZ, ref edwardsZ, ref edwardsY);
            FieldOperations.fe_invert(out tempZ, ref tempZ);
            FieldOperations.fe_mul(out montgomeryX, ref tempX, ref tempZ);
        }
    }
}