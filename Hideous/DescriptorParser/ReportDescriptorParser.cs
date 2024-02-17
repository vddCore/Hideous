namespace Hideous.DescriptorParser
{
    internal class ReportDescriptorParser
    {
        private byte[] _data;
        
        private List<HidReportDescriptor> _reportDescriptors = new();
            
        private Dictionary<GlobalTag, int> _globalTags = new();
        private Dictionary<LocalTag, int> _localTags = new();

        public ReportDescriptorParser(byte[] data)
        {
            _data = data;
        }

        public List<HidReportDescriptor> ParseDescriptors()
        {
            _reportDescriptors.Clear();
            
            DumpDescriptor(_data);
            
            using (var ms = new MemoryStream(_data))
            using (var br = new BinaryReader(ms))
            {
                Parse(br);
            }

            return _reportDescriptors;
        }

        private void DumpDescriptor(byte[] descriptor)
        {
            for (var i = 0; i < descriptor.Length; i++)
            {
                if (i != 0 && i % 16 == 0)
                {
                    Console.Write("\n");
                }

                Console.Write(descriptor[i].ToString("X2"));
                Console.Write(" ");
            }
        }

        // TFW you realize it's easier to just treat this thing as a
        // stack-based virtual machine.
        //
        // Suddenly HID Report Descriptors make sense!
        //
        private void Parse(BinaryReader br)
        {
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var (size, type, tagByte) = ParseItemHeader(br.ReadByte());

                switch (type)
                {
                    case ItemType.Main:
                    {
                        var tag = (MainTag)tagByte; 
                        break;
                    }

                    case ItemType.Global:
                    {
                        var tag = (GlobalTag)tagByte;
                        var value = 0;
                        for (var i = 0; i < size; i++)
                        {
                            value |= (br.ReadByte() << i);
                        }

                        _globalTags[tag] = value;
                        break;
                    }

                    case ItemType.Local:
                    {
                        var tag = (LocalTag)tagByte;
                        break;
                    }
                    
                    case ItemType.Reserved: throw new FormatException("bType field of 0b11 is reserved.");
                    
                    default: throw new FormatException($"bType field of {type} is malformed.");
                }
            }
        }

        private (byte Size, ItemType Type, byte Tag) ParseItemHeader(byte b)
        {
            // HID Specification v1.11 6.2.2.2
            var size = (byte)((b & 0b00000011) switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 4,
                _ => throw new FormatException("Unexpected bSize field in short item header.")
            });

            var type = (ItemType)((b & 0b00001100) >> 2);
            var tag = (byte)((b & 0b11110000) >> 4);

            return (size, type, tag);
        }
    }
}