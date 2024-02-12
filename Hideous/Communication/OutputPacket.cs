namespace Hideous.Communication
{
    public abstract class OutputPacket : Packet<OutputPacket>
    {
        protected OutputPacket(byte reportId, int packetLength, params byte[] data)
            : base(1, packetLength, data)
        {
            Data[0] = reportId;
        }
    }
}