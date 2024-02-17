using System.Collections;
using static Hideous.Native.HidApi;

namespace Hideous
{
    public sealed class HidDeviceCollection : IEnumerable<HidDevice>, IDisposable
    {
        private bool _isDisposed;
        private readonly List<HidDevice> _devices = new();
            
        public ushort VendorID { get; }
        public ushort ProductID { get; }

        public IReadOnlyList<HidDevice> Devices => _devices;

        public HidDeviceCollection()
            : this(0, 0)
        {
        }
        
        public HidDeviceCollection(ushort vendorId, ushort productId)
        {
            VendorID = vendorId;
            ProductID = productId;

            unsafe
            {
                var devices = hid_enumerate(VendorID, ProductID);

                if (devices == null)
                {
                    throw new HidException(
                        $"No devices identified by {vendorId:X4}:{productId:X4} were found on your machine."
                    );
                }
                
                var currentDevice = devices;

                while (currentDevice != null)
                {
                    _devices.Add(new HidDevice(this, *currentDevice));
                    currentDevice = currentDevice->next;
                }

                hid_free_enumeration(devices);
            }
        }

        public IEnumerator<HidDevice> GetEnumerator()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("This device collection has already been disposed.");
            }
            
            return _devices.GetEnumerator();   
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Dispose()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("This device collection has already been disposed.");
            }
            
            foreach (var device in _devices)
            {
                device.Dispose();
            }
            
            _devices.Clear();
            _isDisposed = true;
        }
    }
}