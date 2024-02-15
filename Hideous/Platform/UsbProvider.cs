using Hideous.Communication;

namespace Hideous.Platform
{
    internal abstract class UsbProvider : IDisposable
    {
        protected ushort VendorID { get; }
        protected ushort ProductID { get; }

        protected UsbProvider(ushort vendorId, ushort productId)
        {
            VendorID = vendorId;
            ProductID = productId;
        }

        internal abstract List<DeviceReport> EnumerateDeviceReports();
        
        public abstract void Set(byte[] data);
        public abstract byte[] Get(byte[] data);
        public abstract void Write(byte[] data);
        public abstract byte[] Read(byte[] data);
        
        public abstract void Dispose();
    }
}