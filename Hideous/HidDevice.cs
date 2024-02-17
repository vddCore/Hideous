using static Hideous.Native.HidApi;

namespace Hideous
{
    public sealed class HidDevice
    {
        private bool _isDisposed;
        private IntPtr _connectionHandle;
        
        public HidDeviceCollection Collection { get; private set; }
        public HidDeviceProperties Properties { get; private set; }
        public HidReportDescriptor Descriptor { get; private set; } = null!;
        
        public bool IsConnectionOpen => _connectionHandle != IntPtr.Zero;

        internal HidDevice(HidDeviceCollection collection, hid_device_info info)
        {
            Collection = collection;
            Properties = new HidDeviceProperties(info);
        }

        public void Connect()
        {
            EnsureNotDisposed("Cannot connect: device has been disposed.");
            EnsureNotConnected("Cannot connect: connection is already open.");
            
            _connectionHandle = hid_open_path(Properties.DevicePath);

            if (_connectionHandle == IntPtr.Zero)
            {
                throw new HidException(
                    $"Failed to connect to the specified device: {hid_error()}"
                );
            }

            Descriptor = new HidReportDescriptor(ReadRawReportDescriptor());
        }

        public void Disconnect()
        {
            EnsureNotDisposed("Cannot disconnect: device has been disposed.");
            EnsureConnected("Cannot disconnect: no connection is open.");
            
            hid_close(_connectionHandle);
            _connectionHandle = IntPtr.Zero;
        }

        public byte[] ReadRawReportDescriptor()
        {
            EnsureNotDisposed();
            EnsureConnected();

            var result = hid_get_report_descriptor(_connectionHandle, out var descriptor);
            if (result < 0)
            {
                throw new HidException(
                    $"Unable to retrieve device's raw report descriptor: {hid_error(_connectionHandle)}"
                );
            }

            Array.Resize(ref descriptor, result);
            return descriptor;
        }

        internal void Dispose()
        {
            EnsureNotDisposed(
                "Attempt to dispose a HID device more than once. " +
                "This is an internal error - please report."
            );
            
            Collection = null!;
            Properties = null!;
            Descriptor = null!;

            if (_connectionHandle != IntPtr.Zero)
            {
                Disconnect();
            }

            _isDisposed = true;
        }

        private void EnsureNotDisposed(string? customMsg = null)
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException(
                    customMsg 
                    ?? "This HID device has already been disposed."
                );
            }
        }

        private void EnsureConnected(string? customMsg = null)
        {
            if (_connectionHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    customMsg 
                    ?? "This operation requires a connection to the device to be open."
                );
            }
        }
        
        private void EnsureNotConnected(string? customMsg = null)
        {
            if (_connectionHandle != IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    customMsg
                    ?? "This operation requires the device to be disconnected first."
                );
            }
        }
    }
}