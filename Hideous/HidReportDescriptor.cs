using Hideous.DescriptorParser;

namespace Hideous
{
    public sealed class HidReportDescriptor
    {
        internal HidReportDescriptor(byte[] rawDescriptor)
        {
            var reportDescriptorParser = new ReportDescriptorParser(rawDescriptor);
        }

        private void Parse(BinaryReader br)
        {

        }
    }
}