using System.Runtime.InteropServices;
using System.Text;
using static Hideous.Native.HidApi;

namespace Hideous
{
    public sealed record HidDeviceProperties
    {
        public string DevicePath { get; }
        public ushort VendorID { get; }
        public ushort ProductID { get; }
        public string SerialNumber { get; }
        public ushort VersionNumber { get; }
        public string ManufacturerString { get; }
        public string ProductString { get; }
        public ushort UsagePage { get; }
        public ushort UsageId { get; }
        public int InterfaceNumber { get; }
        public HidBusType BusType { get; }
        
        internal HidDeviceProperties(hid_device_info info)
        {
            DevicePath = Marshal.PtrToStringAnsi(info.path) ?? string.Empty;
            VendorID = info.vendor_id;
            ProductID = info.product_id;
            SerialNumber = DecodeWideCharacterString(info.serial_number) ?? string.Empty;
            VersionNumber = info.release_number;
            ManufacturerString = DecodeWideCharacterString(info.manufacturer_string) ?? string.Empty;
            ProductString = DecodeWideCharacterString(info.product_string) ?? string.Empty;
            UsagePage = info.usage_page;
            UsageId = info.usage;
            InterfaceNumber = info.interface_number;
            BusType = (HidBusType)info.bus_type;
        }

        private string? DecodeWideCharacterString(IntPtr address)
        {
            var bytes = new List<byte>(256);
            
            unsafe
            {
                var addr = (byte*)address;
                
                while (true)
                {
                    if (*(addr + 0) == 0 && *(addr + 1) == 0 && *(addr + 2) == 0 && *(addr + 3) == 0)
                    {
                        break;
                    }
                    
                    bytes.Add(*(addr + 0));
                    bytes.Add(*(addr + 1));
                    bytes.Add(*(addr + 2));
                    bytes.Add(*(addr + 3));
                    
                    addr += 4;
                }
            }

            try
            {
                return Encoding.Unicode.GetString(bytes.ToArray());
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("HID Device Properties {");
            sb.AppendLine($"  System device path: {DevicePath}");
            sb.AppendLine($"             VID:PID: {VendorID:X4}:{ProductID:X4}");
            sb.AppendLine($"       Serial number: {SerialNumber}");
            sb.AppendLine($"      Version number: 0x{VersionNumber:X4} ({VersionNumber})");
            sb.AppendLine($" Manufacturer string: {ManufacturerString}");
            sb.AppendLine($"      Product string: {ProductString}");
            sb.AppendLine($"          Usage page: 0x{UsagePage:X4}");
            sb.AppendLine($"            Usage ID: 0x{UsageId:X4}");
            sb.AppendLine($"    Interface number: {InterfaceNumber}");
            sb.AppendLine($"            Bus type: {BusType}");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }
}