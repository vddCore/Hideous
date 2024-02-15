using Hideous.Communication;
using Hideous.Platform;

namespace Hideous
{
    public abstract class Device : IDisposable
    {
        private Dictionary<byte, DeviceInputReport> _inputs = new();
        private Dictionary<byte, DeviceOutputReport> _outputs = new();
        private Dictionary<byte, DeviceFeatureReport> _features = new();

        internal UsbProvider UsbProvider { get; }

        protected IReadOnlyDictionary<byte, DeviceInputReport> Inputs => _inputs;
        protected IReadOnlyDictionary<byte, DeviceOutputReport> Outputs => _outputs;
        protected IReadOnlyDictionary<byte, DeviceFeatureReport> Features => _features;

        protected Device(DeviceCharacteristics characteristics)
        {
            UsbProvider = new GenericUsbProvider(characteristics);

            var reports = UsbProvider.EnumerateDeviceReports();

            foreach (var report in reports)
            {
                switch (report)
                {
                    case DeviceInputReport inputReport:
                    {
                        _inputs.Add(report.Id, inputReport);
                        break;
                    }

                    case DeviceOutputReport outputReport:
                    {
                        _outputs.Add(report.Id, outputReport);
                        break;
                    }

                    case DeviceFeatureReport featureReport:
                    {
                        _features.Add(report.Id, featureReport);
                        break;
                    }
                }
            }
        }
        
        public void Dispose()
            => UsbProvider.Dispose();
    }
}