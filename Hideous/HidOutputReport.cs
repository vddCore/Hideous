using System.Text;

namespace Hideous
{
    public sealed class HidOutputReport : HidReport
    {
        private readonly HidDevice _device;

        internal HidOutputReport(HidDevice device, int bitsPerField, int fieldCount, ushort usagePage, ushort usageId) 
            : base(bitsPerField, fieldCount, usagePage, usageId)
        {
            _device = device;
        }

        internal HidOutputReport(HidDevice device, byte id, int bitsPerField, int fieldCount, ushort usagePage, ushort usageId) 
            : base(id, bitsPerField, fieldCount, usagePage, usageId)
        {
            _device = device;
        }

        public int Write(byte[] data)
        {
            Array.Resize(ref data, DataLength + 1);
            for (var i = data.Length - 1; i >= 1; i--)
            {
                data[i] = data[i - 1];
            }
                
            data[0] = Id;

            return _device.WriteOutputReport(data) - 1;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("HID Output Report ");
            sb.Append(base.ToString());
            
            return sb.ToString();
        }
    }
}