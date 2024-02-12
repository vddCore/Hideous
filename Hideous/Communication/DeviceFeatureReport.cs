using Hideous.Communication;
using Hideous.Platform;

namespace Hideous
{
    public sealed class DeviceFeatureReport : DeviceReport
    {
        internal DeviceFeatureReport(UsbProvider usbProvider, byte id, IEnumerable<uint> usageValues) 
            : base(usbProvider, id, usageValues)
        {
        }
        
        public void Set(FeaturePacket packet)
        {
            if (packet.Data[0] != Id)
            {
                throw new HideousException(
                    $"Report ID mismatch. Report: {Id}, packet: '{packet.Data[0]}'.");
            }
            
            UsbProvider.Set(packet.Data);
        }

        public byte[] Get(FeaturePacket packet)
        {
            if (packet.Data[0] != Id)
            {
                throw new HideousException(
                    $"Report ID mismatch. Report ID is '{Id}', while packet declares ID '{packet.Data[0]}'."
                );
            }

            return UsbProvider.Get(packet.Data);
        }
    }
}