using Hideous.Internal;
using Hideous.Platform;

namespace Hideous.Communication
{
    public sealed class DeviceOutputReport : DeviceReport
    {
        internal DeviceOutputReport(UsbProvider usbProvider, byte id, int length, IEnumerable<uint> usageValues) 
            : base(usbProvider, id, length, usageValues)
        {
        }
        
        public void Set(byte[] reportData)
        {
            Throw.If.DataBufferTooLong(Length, reportData.Length);
            
            var report = new byte[reportData.Length + 1];

            report[0] = Id;
            Buffer.BlockCopy(reportData, 0, report, 1, reportData.Length);

            UsbProvider.Write(report);
        }
        
        public void Set<T>(T request)
            where T : Request
        {
            Throw.If.ReportIdMismatch(Id, request.ReportId, nameof(request));
            Throw.If.ReportBufferTooLong(Length, request.Data.Length, nameof(request));

            UsbProvider.Write(request.Data);
        }
        
        public void RawSet(byte[] report)
        {
            Throw.If.ReportIdMismatch(Id, report[0], nameof(report));
            Throw.If.ReportBufferTooLong(Length, report.Length, nameof(report));

            UsbProvider.Write(report);
        }
    }
}