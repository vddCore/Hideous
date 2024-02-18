namespace Hideous.DescriptorParser
{
    // HID Specification v1.11 6.2.2.4, Parts table
    public record ItemFlags(int Flags)
    {
        public bool Constant => (Flags & 0b00000000_00000001) != 0;
        public bool Data => !Constant;

        public bool Variable => (Flags & 0b00000000_00000010) != 0;
        public bool Array => !Variable;

        public bool Relative => (Flags & 0b00000000_00000100) != 0;
        public bool Absolute => !Relative;

        public bool Wrap => (Flags & 0b00000000_00001000) != 0;
        public bool NoWrap => !Wrap;

        public bool NonLinear => (Flags & 0b00000000_00010000) != 0;
        public bool Linear => !NonLinear;

        public bool PreferredState => (Flags & 0b00000000_00100000) != 0;
        public bool NoPreferred => !PreferredState;
        
        public bool NoNullPosition => (Flags & 0b00000000_01000000) != 0;
        public bool NullState => !NoNullPosition;
        
        /* Invalid for Input items (always 0) */
        public bool NonVolatile => (Flags & 0b00000000_10000000) != 0;
        public bool Volatile => !NonVolatile;

        public bool BufferedBytes => (Flags & 0b00000001_00000000) != 0;
        public bool BitField => !BufferedBytes;
    }
}