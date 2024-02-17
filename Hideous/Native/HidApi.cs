using System.Runtime.InteropServices;

namespace Hideous.Native
{
    internal static class HidApi
    {
        private const string LibraryName = "hidapi";

        public const int HID_API_MAX_REPORT_DESCRIPTOR_SIZE = 4096;
        
        public enum hid_bus_type
        {
            HID_API_BUS_UNKNOWN = 0x00,
            HID_API_BUS_USB = 0x01,
            HID_API_BUS_BLUETOOTH = 0x02,
            HID_API_BUS_I2C = 0x03,
            HID_API_BUS_SPI = 0x04
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct hid_api_version
        {
            public int major;
            public int minor;
            public int patch;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct hid_device_info
        {
            public /* char* */ IntPtr path;
            public ushort vendor_id;
            public ushort product_id;
            public /* wchar_t* */ IntPtr serial_number;
            public ushort release_number;
            public /* wchar_t* */ IntPtr manufacturer_string;
            public /* wchar_t* */ IntPtr product_string;
            public ushort usage_page;
            public ushort usage;
            public int interface_number;
            public unsafe hid_device_info* next;
            public hid_bus_type bus_type;
        }

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_init();
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_exit();

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe hid_device_info* hid_enumerate(
            ushort vendor_id, 
            ushort product_id
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void hid_free_enumeration(hid_device_info* devs);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* hid_device* */ hid_open(
            ushort vendor_id,
            ushort product_id,
            [In, MarshalAs(UnmanagedType.LPWStr)] string? serial_number
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* hid_device* */ hid_open_path(
            [In, MarshalAs(UnmanagedType.LPStr)] string path
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_write(
            IntPtr dev,
            byte* data,
            int length
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_read_timeout(
            IntPtr dev,
            byte* data,
            int length,
            int milliseconds
        );
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_read(
            IntPtr dev,
            byte* data,
            int length
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_set_nonblocking(IntPtr dev, bool nonblock);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_send_feature_report(
            IntPtr dev,
            byte* data,
            uint length
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_get_feature_report(
            IntPtr dev,
            byte* data,
            uint length
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int hid_get_input_report(
            IntPtr dev,
            byte* data,
            uint length
        );

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void hid_close(IntPtr dev);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_get_manufacturer_string")]
        private static extern int hid_get_manufacturer_string_INTERNAL(IntPtr dev, IntPtr str, int maxlen);

        public static int hid_get_manufacturer_string(IntPtr dev, out string str, int maxlen = 256)
            => common_unicode_marshal(dev, out str, maxlen, hid_get_manufacturer_string_INTERNAL);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_get_product_string")]
        private static extern int hid_get_product_string_INTERNAL(IntPtr dev, IntPtr str, int maxlen);

        public static int hid_get_product_string(IntPtr dev, out string str, int maxlen = 256)
            => common_unicode_marshal(dev, out str, maxlen, hid_get_product_string_INTERNAL);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_get_serial_number_string")]
        private static extern int hid_get_serial_number_string_INTERNAL(IntPtr dev, IntPtr str, int maxlen);

        public static int hid_get_serial_number_string(IntPtr dev, out string str, int maxlen = 256)
            => common_unicode_marshal(dev, out str, maxlen, hid_get_serial_number_string_INTERNAL);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe hid_device_info* hid_get_device_info(IntPtr dev);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_get_indexed_string")]
        private static extern int hid_get_indexed_string(IntPtr dev, int string_index, IntPtr str, int maxlen);

        public static int hid_get_indexed_string(IntPtr dev, int string_index, out string str, int maxlen = 256)
        {
            return common_unicode_marshal(
                dev,
                out str,
                maxlen, 
                (dev, buffer, maxlen) => hid_get_indexed_string(dev, string_index, buffer, maxlen)
            );
        }

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_get_report_descriptor")]
        private static extern unsafe int hid_get_report_descriptor_INTERNAL(IntPtr dev, byte* array, int maxlen);

        public static int hid_get_report_descriptor(IntPtr dev, out byte[] array, int maxlen = HID_API_MAX_REPORT_DESCRIPTOR_SIZE)
        {
            array = new byte[maxlen];

            var result = -1;
            
            unsafe
            {
                fixed (byte* data = array)
                {
                    result = hid_get_report_descriptor_INTERNAL(dev, data, maxlen);
                }
            }

            return result;
        }

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_error")]
        private static extern IntPtr hid_error_INTERNAL(IntPtr dev);

        public static string hid_error(IntPtr dev)
        {
            return Marshal.PtrToStringUni(
                hid_error_INTERNAL(dev)
            ) ?? string.Empty;
        }

        public static string hid_error() => hid_error(IntPtr.Zero);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe hid_api_version* hid_version();
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_version_str")]
        private static extern IntPtr hid_version_str_INTERNAL();

        public static string hid_version_str()
        {
            return Marshal.PtrToStringAnsi(
                hid_version_str_INTERNAL()
            ) ?? string.Empty;
        }
        
        private static int common_unicode_marshal(
            IntPtr dev,
            out string str,
            int maxlen, 
            Func<IntPtr, IntPtr, int, int> internal_func)
        {
            str = string.Empty;
            var buffer = Marshal.AllocHGlobal(new IntPtr(maxlen * 2));
            var result = internal_func(dev, buffer, maxlen);

            if (result == 0)
            {
                str = Marshal.PtrToStringUni(buffer, maxlen);
            }

            Marshal.FreeHGlobal(buffer);
            return result;
        }
    }
}