using static Hideous.Native.HidApi;

namespace Hideous
{
    public enum HidBusType
    {
        Unknown = hid_bus_type.HID_API_BUS_UNKNOWN,
        USB = hid_bus_type.HID_API_BUS_USB,
        Bluetooth = hid_bus_type.HID_API_BUS_BLUETOOTH,
        I2C = hid_bus_type.HID_API_BUS_I2C,
        SPI = hid_bus_type.HID_API_BUS_SPI
    }
}