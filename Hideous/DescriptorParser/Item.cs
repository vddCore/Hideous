
namespace Hideous.DescriptorParser
{
    internal class Item
    {
        private readonly ItemType _type;
        private readonly byte _tag;
        private readonly byte _size;
        private readonly int? _value;
        private readonly List<Item> _items = new();
        
        public ItemType Type => _type;
        public byte Size => _size;
        public int? Value => _value;
        public IReadOnlyList<Item> Items => _items;
        
        public Item? this[GlobalTag tag] 
            => Items.FirstOrDefault(x => x.Type == ItemType.Global && x.GlobalTag == tag);

        public Item? this[LocalTag tag] 
            => Items.FirstOrDefault(x => x.Type == ItemType.Local && x.LocalTag == tag);

        public MainTag MainTag
        {
            get
            {
                EnsureItemOfType(ItemType.Main);
                return (MainTag)_tag;
            }
        }
        
        public GlobalTag GlobalTag
        {
            get
            {
                EnsureItemOfType(ItemType.Global);
                return (GlobalTag)_tag;
            }
        }
        
        public LocalTag LocalTag
        {
            get
            {
                EnsureItemOfType(ItemType.Local);
                return (LocalTag)_tag;
            }
        }

        private Item(ItemType type, byte tag, byte size)
        {
            _type = type;
            _tag = tag;
            _size = size;
        }

        private Item(ItemType type, byte tag, byte size, int value)
            : this(type, tag, size)
        {
            _value = value;
        }

        public Item(MainTag tag, byte size)
            : this(ItemType.Main, (byte)tag, size)
        {
        }
        
        public Item(MainTag tag, byte size, int value)
            : this(ItemType.Main, (byte)tag, size, value)
        {
        }

        public Item(GlobalTag tag, byte size, int value)
            : this(ItemType.Global, (byte)tag, size, value)
        {
        }

        public Item(LocalTag tag, byte size, int value)
            : this(ItemType.Local, (byte)tag, size, value)
        {
        }

        public IEnumerable<Item> GetItems(GlobalTag tag)
            => Items.Where(x => x.Type == ItemType.Global && x.GlobalTag == tag);

        public IEnumerable<Item> GetItems(LocalTag tag)
            => Items.Where(x => x.Type == ItemType.Local && x.LocalTag == tag);
        
        public void AddItem(Item item)
            => _items.Add(item);

        private void EnsureItemOfType(ItemType type)
        {
            if (Type != type)
            {
                throw new InvalidOperationException($"This operation is only supported for {type.ToString().ToLower()} items.");
            }
        }
    }
}