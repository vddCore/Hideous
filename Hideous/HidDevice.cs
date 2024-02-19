using static Hideous.Native.HidApi;

namespace Hideous
{
    public sealed class HidDevice
    {
        private HidReportDescriptor? _descriptor;

        private bool _isReadBlocking;
        private bool _isDisposed;
        private IntPtr _connectionHandle;

        public HidDeviceCollection Collection { get; private set; }
        public HidDeviceProperties Properties { get; private set; }

        public HidReportDescriptor Descriptor => _descriptor ?? throw new InvalidOperationException(
            "No descriptor for this device is present yet - it will be retrieved when a connection is established to the device."
        );

        public bool IsConnectionOpen => _connectionHandle != IntPtr.Zero;

        public bool IsReadBlocking
        {
            get
            {
                EnsureNotDisposed();

                return _isReadBlocking;
            }

            set
            {
                EnsureNotDisposed();

                _isReadBlocking = value;

                if (IsConnectionOpen)
                {
                    hid_set_nonblocking(_connectionHandle, !_isReadBlocking);
                }
            }
        }

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

            _descriptor = new HidReportDescriptor(
                this,
                ReadRawReportDescriptor()
            );

            hid_set_nonblocking(_connectionHandle, !_isReadBlocking);
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
                    $"Unable to retrieve the device's raw report descriptor: {hid_error(_connectionHandle)}"
                );
            }

            Array.Resize(ref descriptor, result);
            return descriptor;
        }

        public int WriteOutputReport(byte[] rawReport)
        {
            EnsureNotDisposed();
            EnsureConnected();

            unsafe
            {
                fixed (byte* ptr = rawReport)
                {
                    var result = hid_write(
                        _connectionHandle,
                        ptr,
                        rawReport.Length
                    );

                    if (result < 0)
                    {
                        throw new HidException(
                            $"Error while writing an output report to the device: {hid_error(_connectionHandle)}"
                        );
                    }

                    return result;
                }
            }
        }

        public int ReadInputReport(byte[] rawReport, bool blocking = false)
        {
            EnsureNotDisposed();
            EnsureConnected();

            unsafe
            {
                fixed (byte* ptr = rawReport)
                {
                    int result;

                    if (blocking)
                    {
                        result = hid_read_timeout(
                            _connectionHandle,
                            ptr,
                            rawReport.Length,
                            -1 /* hid_read_timeout will issue a blocking wait if this is -1 */
                        );
                    }
                    else
                    {
                        result = hid_read(
                            _connectionHandle,
                            ptr,
                            rawReport.Length
                        );
                    }

                    if (result < 0)
                    {
                        throw new HidException(
                            $"Error while reading an input report from the device: {hid_error(_connectionHandle)}"
                        );
                    }

                    return result;
                }
            }
        }

        public int ReadInputReport(byte[] rawReport, TimeSpan timeout)
        {
            EnsureNotDisposed();
            EnsureConnected();

            unsafe
            {
                fixed (byte* ptr = rawReport)
                {
                    var result = hid_read_timeout(
                        _connectionHandle,
                        ptr,
                        rawReport.Length,
                        (int)timeout.TotalMilliseconds
                    );

                    if (result < 0)
                    {
                        throw new HidException(
                            $"Error while reading an input report from the device: {hid_error(_connectionHandle)}"
                        );
                    }

                    return result;
                }
            }
        }

        public int SetFeatureReport(byte[] rawReport)
        {
            EnsureNotDisposed();
            EnsureConnected();

            unsafe
            {
                fixed (byte* ptr = rawReport)
                {
                    var result = hid_send_feature_report(
                        _connectionHandle,
                        ptr,
                        rawReport.Length
                    );
                    
                    if (result < 0)
                    {
                        throw new HidException(
                            $"Error while sending a feature report to the device: {hid_error(_connectionHandle)}"
                        );
                    }

                    return result;
                }
            }
        }
        
        public int GetFeatureReport(byte[] rawReport)
        {
            EnsureNotDisposed();
            EnsureConnected();

            unsafe
            {
                fixed (byte* ptr = rawReport)
                {
                    var result = hid_get_feature_report(
                        _connectionHandle,
                        ptr,
                        rawReport.Length
                    );
                    
                    if (result < 0)
                    {
                        throw new HidException(
                            $"Error while retrieving a feature report from the device: {hid_error(_connectionHandle)}"
                        );
                    }

                    return result;
                }
            }
        }

        internal void Dispose()
        {
            EnsureNotDisposed(
                "Attempt to dispose a HID device more than once. " +
                "This is an internal error - please report."
            );

            Collection = null!;
            Properties = null!;
            _descriptor = null!;

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