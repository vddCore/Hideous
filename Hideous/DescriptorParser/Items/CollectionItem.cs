namespace Hideous.DescriptorParser.Items
{
    internal sealed class CollectionItem : Item
    {       
        public CollectionItem? Parent { get; }
        public CollectionType CollectionType { get; }

        public Item? this[MainTag tag] 
            => Items.FirstOrDefault(x => x.Type == ItemType.Main && x.MainTag == tag);

        public CollectionItem(CollectionType type, CollectionItem? parent)
            : base(MainTag.Collection, 0)
        {
            CollectionType = type;
            Parent = parent;
        }

        public IEnumerable<Item> GetItems(MainTag tag)
            => Items.Where(x => x.Type == ItemType.Main && x.MainTag == tag);
    }
}