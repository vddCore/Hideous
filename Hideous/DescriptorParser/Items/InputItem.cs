namespace Hideous.DescriptorParser.Items
{
    internal sealed class InputItem : Item
    {
        public ItemFlags Flags { get; }

        public InputItem(byte size, int value) 
            : base(MainTag.Input, size, value)
        {
            Flags = new ItemFlags(value);
        }
    }
}