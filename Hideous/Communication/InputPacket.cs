namespace Hideous.Communication
{
    public abstract class InputPacket : Packet<InputPacket>
    {
        protected InputPacket(byte reportId, int packetLength, params byte[] data)
            : base(1, packetLength, data)
        {
            Data[0] = reportId;
        }
    }
}