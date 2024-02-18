namespace Hideous.DescriptorParser.Items
{
    internal sealed class FeatureItem : Item
    {
        public ItemFlags Flags { get; }

        public FeatureItem(byte size, int value) 
            : base(MainTag.Feature, size, value)
        {
            Flags = new ItemFlags(value);
        }
    }
}