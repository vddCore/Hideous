namespace Hideous.DescriptorParser
{
    // HID Specification v1.11: 6.2.2.2
    internal enum ItemType : byte
    {
        Main     = 0,
        Global   = 1,
        Local    = 2,
        Reserved = 3
    }
}