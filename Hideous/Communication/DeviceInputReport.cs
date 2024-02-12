using Hideous.Communication;
using Hideous.Platform;

namespace Hideous
{
    public sealed class DeviceInputReport : DeviceReport
    {
        internal DeviceInputReport(UsbProvider usbProvider, byte id, IEnumerable<uint> usageValues) 
            : base(usbProvider, id, usageValues)
        {
        }
        
        public byte[] Get(InputPacket packet)
        {
            if (packet.Data[0] != Id)
            {
                throw new HideousException(
                    $"Report ID mismatch. Report ID is '{Id}', while packet declares ID '{packet.Data[0]}'."
                );
            }

            return UsbProvider.Read(packet.Data);
        }
    }
}