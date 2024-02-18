namespace Hideous.DescriptorParser.Items
{
    internal sealed class OutputItem : Item
    {
        public ItemFlags Flags { get; }

        public OutputItem(byte size, int value) 
            : base(MainTag.Output, size, value)
        {
            Flags = new ItemFlags(value);
        }
    }
}