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

        internal HidInputReport(HidDevice device, byte id, int bitsPerField, int fieldCount, ushort usagePage,
            ushort usageId)
            : base(id, bitsPerField, fieldCount, usagePage, usageId)
        {
            _device = device;
        }

        public byte[] Read(byte[] data, bool blocking = false)
        {
            Array.Resize(ref data, DataLength + 1);
            for (var i = data.Length - 1; i >= 1; i--)
            {
                data[i] = data[i - 1];
            }
            
            data[0] = Id;

            var count = _device.ReadInputReport(data, blocking);
            var ret = new byte[count];

            if (count > 0)
            {
                Array.Copy(data, 0, ret, 0, count);
            }
            
            return ret;
        }

        public byte[] Read(byte[] data, TimeSpan timeout)
        {
            Array.Resize(ref data, DataLength + 1);
            for (var i = data.Length - 1; i >= 1; i--)
            {
                data[i] = data[i - 1];
            }

            data[0] = Id;

            var count = _device.ReadInputReport(data, timeout);
            var ret = new byte[count];

            if (count > 0)
            {
                Array.Copy(data, 1, ret, 0, count - 1);
            }

            return ret;
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