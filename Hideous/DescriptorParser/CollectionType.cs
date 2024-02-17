namespace Hideous.DescriptorParser
{
    // HID Specification v1.11: 6.2.2.6
    internal enum CollectionType : byte
    {
        Physical      = 0x00,
        Application   = 0x01, 
        Logical       = 0x02,
        Report        = 0x03,
        NamedArray    = 0x04,
        UsageSwitch   = 0x05,
        UsageModifier = 0x06
    }
}