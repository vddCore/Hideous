namespace Hideous.DescriptorParser
{
    // HID Specification v1.11: 6.2.2.4
    internal enum MainTag : byte
    {
        Input         = 0b1000,
        Output        = 0b1001,
        Feature       = 0b1011,
        Collection    = 0b1010,
        EndCollection = 0b1100
    }
}