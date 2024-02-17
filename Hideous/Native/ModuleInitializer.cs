using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hideous.Native
{
    internal static class ModuleInitializer
    {
        private static bool UsingLibUsbBackend { get; set; }
            = OperatingSystem.IsLinux() && Environment.GetCommandLineArgs().Contains("--hideous-libusb");

        private static string HidApiExtractionFileName
        {
            get
            {
                if (OperatingSystem.IsLinux())
                {
                    return "hidapi.so";
                }

                if (OperatingSystem.IsWindows())
                {
                    return "hidapi.dll";
                }

                ThrowPlatformNotSupported();
                return string.Empty;
            }
        }

        private static string EmbeddedHidApiFileName
        {
            get
            {
                if (OperatingSystem.IsLinux())
                {
                    return UsingLibUsbBackend
                        ? "libhidapi-libusb.so"
                        : "libhidapi-hidraw.so";
                }

                if (OperatingSystem.IsWindows())
                {
                    return "hidapi.dll";
                }

                return string.Empty;
            }
        }

        private static string EmbeddedLibUsbFileName => "libusb-1.0.so";

        [ModuleInitializer]
        internal static void Initialize()
        {
            EnsurePlatformSupported();
            ExtractEmbeddedLibraryIfRequired();
        }

        private static void ExtractEmbeddedLibraryIfRequired()
        {
            var appBasePath = AppContext.BaseDirectory;
            var targetLibraryPath = Path.Combine(appBasePath, HidApiExtractionFileName);

            if (!File.Exists(targetLibraryPath))
            {
                using (var libStream = GetEmbeddedHidApiResourceStream())
                using (var fs = File.OpenWrite(targetLibraryPath))
                {
                    libStream.CopyTo(fs);
                }
            }

            ExtractLibUsbIfApplicable(appBasePath);
        }

        private static void ExtractLibUsbIfApplicable(string appBasePath)
        {
            if (OperatingSystem.IsLinux() && UsingLibUsbBackend)
            {
                var targetLibUsbPath = Path.Combine(appBasePath, EmbeddedLibUsbFileName);

                if (!File.Exists(targetLibUsbPath))
                {
                    using(var libUsbStream = GetEmbeddedLibUsbResourceStream())
                    using (var fs = File.OpenWrite(targetLibUsbPath))
                    {
                        libUsbStream.CopyTo(fs);   
                    }   
                }
            }
        }

        private static Stream GetEmbeddedHidApiResourceStream()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(GetEmbeddedHidApiResourcePath())!;
        }
        
        private static Stream GetEmbeddedLibUsbResourceStream()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(GetEmbeddedLibUsbResourcePath())!;
        }

        private static string GetEmbeddedHidApiResourcePath()
        {
            if (OperatingSystem.IsLinux())
            {
                return $"Hideous.Native.Binaries.linux_x64.{EmbeddedHidApiFileName}";
            }

            if (OperatingSystem.IsWindows())
            {
                return $"Hideous.Native.Binaries.win_x64.{EmbeddedHidApiFileName}";
            }

            ThrowPlatformNotSupported();
            /* dummy */ return null;
        }
        
        private static string GetEmbeddedLibUsbResourcePath()
        {
            if (OperatingSystem.IsLinux())
            {
                return $"Hideous.Native.Binaries.linux_x64.{EmbeddedLibUsbFileName}";
            }

            ThrowPlatformNotSupported();
            /* dummy */ return null;
        }

        private static void EnsurePlatformSupported()
        {
            if (OperatingSystem.IsLinux() || OperatingSystem.IsWindows())
            {
                return;
            }

            ThrowPlatformNotSupported();
        }
        
        [DoesNotReturn]
        private static void ThrowPlatformNotSupported()
        {
            throw new PlatformNotSupportedException("Your platform is not supported by Hideous.");
        }
    }
}