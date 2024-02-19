using System.Text;

namespace Hideous
{
    public sealed class HidInputReport : HidReport
    {
        private readonly HidDevice _device;
        
        internal HidInputReport(HidDevice device, int bitsPerField, int fieldCount, ushort usagePage, ushort usageId) 
            : base(bitsPerField, fieldCount, usagePage, usageId)
        {
            _device = device;
        }
        
        internal HidInputReport(HidDevice device, byte reportId, int bitsPerField, int fieldCount, ushort usagePage, ushort usageId) 
            : base(reportId, bitsPerField, fieldCount, usagePage, usageId)
        {
            _device = device;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("HID Input Report ");
            sb.Append(base.ToString());
            
            return sb.ToString();
        }
    }
}