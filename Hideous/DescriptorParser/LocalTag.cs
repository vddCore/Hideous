namespace Hideous.DescriptorParser
{
    // HID Specification v1.11: 6.2.2.8
    internal enum LocalTag : byte
    {
        Usage             = 0b0000,
        UsageMinimum      = 0b0001,
        UsageMaximum      = 0b0010,
        DesignatorIndex   = 0b0011,
        DesignatorMinimum = 0b0100,
        DesignatorMaximum = 0b0101,
        StringIndex       = 0b0111,
        StringMinimum     = 0b1000,
        StringMaximum     = 0b1001,
        Delimiter         = 0b1010
    }
}