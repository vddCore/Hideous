using System.ComponentModel;
using HidSharp;
using HidSharp.Reports;

namespace Hideous.Platform
{
    internal class GenericUsbProvider : UsbProvider
    {
        protected HidDevice HidDevice { get; }
        protected HidStream HidStream { get; }

        internal ReportDescriptor ReportDescriptor { get; }

        public GenericUsbProvider(DeviceCharacteristics characteristics)
            : base(characteristics.VendorID, characteristics.ProductID)
        {
            try
            {
                var hidDevices = DeviceList.Local.GetHidDevices(VendorID, ProductID);
                HidDevice = hidDevices.First(characteristics.SatisfiedBy);
            }
            catch (UnauthorizedAccessException uae)
            {
                if (OperatingSystem.IsLinux())
                {
                    throw new HideousException(
                        "Unable to access UDEV.",
                        uae
                    );
                }

                throw;
            }
            catch (Exception e)
            {
                throw new HideousException(
                    "The requested HID device satisfying the provided characteristics was not found on your machine.",
                    e
                );
            }

            ReportDescriptor = HidDevice.GetReportDescriptor();

            var config = new OpenConfiguration();
            config.SetOption(OpenOption.Interruptible, true);
            config.SetOption(OpenOption.Exclusive, false);
            config.SetOption(OpenOption.Priority, 10);

            HidStream = HidDevice.Open(config);
        }

        internal override List<DeviceReport> EnumerateDeviceReports()
        {
            var reports = new List<DeviceReport>();

            foreach (var report in ReportDescriptor.Reports)
            {
                var reportId = report.ReportID;
                var usageValues = report.GetAllUsages();

                switch (report.ReportType)
                {
                    case ReportType.Input:
                    {
                        reports.Add(new DeviceInputReport(this, reportId, usageValues));
                        break;
                    }

                    case ReportType.Output:
                    {
                        reports.Add(new DeviceOutputReport(this, reportId, usageValues));
                        break;
                    }

                    case ReportType.Feature:
                    {
                        reports.Add(new DeviceFeatureReport(this, reportId, usageValues));
                        break;
                    }
                }
            }

            return reports;
        }

        public override void Set(byte[] data)
        {
            WrapException(() =>
            {
                HidStream.SetFeature(data);
                HidStream.Flush();
            });
        }

        public override byte[] Get(byte[] data)
        {
            var outData = new byte[data.Length];
            Array.Copy(data, outData, data.Length);

            WrapException(() =>
            {
                HidStream.GetFeature(outData);
                HidStream.Flush();
            });

            return outData;
        }

        public override void Write(byte[] data)
        {
            WrapException(() =>
            {
                HidStream.Write(data);
                HidStream.Flush();
            });
        }

        public override byte[] Read(byte[] data)
        {
            var inData = new byte[data.Length];
            Array.Copy(data, inData, data.Length);
            
            WrapException(() =>
            {
                HidStream.Read(inData);
            });

            return inData;
        }

        public override void Dispose()
        {
            HidStream.Dispose();
        }

        private void WrapException(Action action)
        {
            try
            {
                action();
            }
            catch (IOException e)
            {
                if (e.InnerException is Win32Exception w32e)
                {
                    if (w32e.NativeErrorCode != 0)
                    {
                        throw;
                    }
                }
            }
        }
    }
}