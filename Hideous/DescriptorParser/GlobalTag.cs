namespace Hideous.DescriptorParser
{
    // HID Specification v1.11: 6.2.2.7
    internal enum GlobalTag : byte
    {
        UsagePage       = 0b0000,
        LogicalMinimum  = 0b0001,
        LogicalMaximum  = 0b0010,
        PhysicalMinimum = 0b0011,
        PhysicalMaximum = 0b0100,
        UnitExponent    = 0b0101,
        Unit            = 0b0110,
        ReportSize      = 0b0111,
        ReportID        = 0b1000,
        ReportCount     = 0b1001,
        Push            = 0b1010,
        Pop             = 0b1011
    }
}