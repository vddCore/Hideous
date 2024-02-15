using Hideous.Internal;
using Hideous.Platform;

namespace Hideous.Communication
{
    public sealed class DeviceInputReport : DeviceReport
    {
        internal DeviceInputReport(UsbProvider usbProvider, byte id, int length, IEnumerable<uint> usageValues) 
            : base(usbProvider, id, length, usageValues)
        {
        }
        
        public byte[] Get(byte[] reportData)
        {
            Throw.If.DataBufferTooLong(Length, reportData.Length);

            var report = new byte[reportData.Length + 1];

            report[0] = Id;
            Buffer.BlockCopy(reportData, 0, report, 1, reportData.Length);

            return UsbProvider.Read(report).Skip(1).ToArray();
        }
        
        public U? Get<T, U>(T request)
            where T : Request
            where U : Response
        {
            Throw.If.ReportIdMismatch(Id, request.ReportId, nameof(request));
            Throw.If.ReportBufferTooLong(Length, request.Data.Length, nameof(request));

            var data = UsbProvider.Read(request.Data);

            try
            {
                return (U?)Activator.CreateInstance(typeof(U), new object[] { data });
            }
            catch
            {
                return null;
            }
        }
        
        public byte[] RawGet(byte[] report)
        {
            Throw.If.ReportIdMismatch(Id, report[0], nameof(report));
            Throw.If.ReportBufferTooLong(Length, report.Length, nameof(report));

            return UsbProvider.Read(report);
        }
    }
}