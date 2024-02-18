using Hideous.DescriptorParser;

namespace Hideous
{
    public sealed class HidReportDescriptor
    {
        internal HidReportDescriptor(byte[] rawDescriptor)
        {
            var descriptorReader = new DescriptorReader(rawDescriptor);
            var tlc = descriptorReader.ReadRawDescriptor();

            Console.WriteLine();
        }
    }
}