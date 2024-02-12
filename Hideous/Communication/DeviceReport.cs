using Hideous.Platform;

namespace Hideous
{
    public abstract class DeviceReport
    {
        private readonly List<(ushort UsagePage, ushort UsageId)> _usages = new();
        
        internal UsbProvider UsbProvider { get; }
        
        public byte Id { get; }
        public IReadOnlyList<(ushort UsagePage, ushort UsageId)> Usages => _usages;

        internal DeviceReport(UsbProvider usbProvider, byte id, IEnumerable<uint> usageValues)
        {
            UsbProvider = usbProvider;
            Id = id;

            foreach (var usageValue in usageValues)
            {
                _usages.Add((
                    /* UsagePage: */ (ushort)((usageValue & 0xFFFF0000) >> 0x10),
                    /*   UsageId: */ (ushort)((usageValue & 0x0000FFFF) >> 0x00)
                ));
            }
        }
    }
}