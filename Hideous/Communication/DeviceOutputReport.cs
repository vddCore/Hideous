using Hideous.Communication;
using Hideous.Platform;

namespace Hideous
{
    public sealed class DeviceOutputReport : DeviceReport
    {
        internal DeviceOutputReport(UsbProvider usbProvider, byte id, IEnumerable<uint> usageValues) 
            : base(usbProvider, id, usageValues)
        {
        }
        
        public void Set(OutputPacket packet)
        {
            if (packet.Data[0] != Id)
            {
                throw new HideousException(
                    $"Report ID mismatch. Report ID is '{Id}', while packet declares ID '{packet.Data[0]}'."
                );
            }

            UsbProvider.Write(packet.Data);
        }
    }
}