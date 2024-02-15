using Hideous.Internal;
using Hideous.Platform;

namespace Hideous.Communication
{
    public sealed class DeviceFeatureReport : DeviceReport
    {
        internal DeviceFeatureReport(UsbProvider usbProvider, byte id, int length, IEnumerable<uint> usageValues)
            : base(usbProvider, id, length, usageValues)
        {
        }

        public void Set(byte[] reportData)
        {
            Throw.If.DataBufferTooLong(Length, reportData.Length);
            
            var report = new byte[reportData.Length + 1];

            report[0] = Id;
            Buffer.BlockCopy(reportData, 0, report, 1, reportData.Length);

            UsbProvider.Set(report);
        }

        public byte[] Get(byte[] reportData)
        {
            Throw.If.DataBufferTooLong(Length, reportData.Length);

            var report = new byte[reportData.Length + 1];

            report[0] = Id;
            Buffer.BlockCopy(reportData, 0, report, 1, reportData.Length);

            return UsbProvider.Get(report).Skip(1).ToArray();
        }

        public void Set<T>(T request)
            where T : Request
        {
            Throw.If.ReportIdMismatch(Id, request.ReportId, nameof(request));
            Throw.If.ReportBufferTooLong(Length, request.Data.Length, nameof(request));

            UsbProvider.Set(request.Data);
        }

        public U? Get<T, U>(T request)
            where T : Request
            where U : Response
        {
            Throw.If.ReportIdMismatch(Id, request.ReportId, nameof(request));
            Throw.If.ReportBufferTooLong(Length, request.Data.Length, nameof(request));

            var data = UsbProvider.Get(request.Data);

            try
            {
                return (U?)Activator.CreateInstance(typeof(U), new object[] { data });
            }
            catch
            {
                return null;
            }
        }

        public void RawSet(byte[] report)
        {
            Throw.If.ReportIdMismatch(Id, report[0], nameof(report));
            Throw.If.ReportBufferTooLong(Length, report.Length, nameof(report));

            UsbProvider.Set(report);
        }

        public byte[] RawGet(byte[] report)
        {
            Throw.If.ReportIdMismatch(Id, report[0], nameof(report));
            Throw.If.ReportBufferTooLong(Length, report.Length, nameof(report));

            return UsbProvider.Get(report);
        }
    }
}