namespace BeaconSdk.Infrastructure.Cryptography.Libsodium
{
    using System;

    public static class SodiumCore
    {
        private static bool isInit;

        /// <summary>Initialize libsodium.</summary>
        /// <remarks>This only needs to be done once, so this prevents repeated calls.</remarks>
        public static void Init()
        {
            if (isInit)
                return;

            SodiumLibrary.sodium_init();
            if (SodiumLibrary.sodium_init() < 0)
                throw new Exception("SodiumLibrary couldn't be initialized, it is not safe to use");

            isInit = true;
        }
    }

}