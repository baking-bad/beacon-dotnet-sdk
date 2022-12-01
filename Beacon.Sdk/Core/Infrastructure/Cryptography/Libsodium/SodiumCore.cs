using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium
{
    public static partial class SodiumCore
    {
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
                if (SodiumLibrary.sodium_library_version_major() != SodiumLibrary.SODIUM_LIBRARY_VERSION_MAJOR ||
                    SodiumLibrary.sodium_library_version_minor() != SodiumLibrary.SODIUM_LIBRARY_VERSION_MINOR)
                {
                    string? version = Marshal.PtrToStringAnsi(SodiumLibrary.sodium_version_string());
                    throw (version != null && version != SodiumLibrary.SODIUM_VERSION_STRING)
                        ? new NotSupportedException($"An error occurred while initializing cryptographic primitives. (Expected libsodium {SodiumLibrary.SODIUM_VERSION_STRING} but found {version}.)")
                        : new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                if (SodiumLibrary.sodium_set_misuse_handler(s_misuseHandler) != 0)
                {
                    throw new NotSupportedException("An error occurred while initializing cryptographic primitives.");
                }

                // sodium_init() returns 0 on success, -1 on failure, and 1 if the library had already been initialized.
                if (SodiumLibrary.sodium_init() < 0)
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
    }
}