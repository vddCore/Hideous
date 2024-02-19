using Hideous.DescriptorParser.Items;

namespace Hideous.DescriptorParser
{
    internal class DescriptorReader
    {
        private byte[] _data;

        private List<CollectionItem> _topLevelCollections = new();
        private Stack<CollectionItem> _collectionDefinitionStack = new();

        private Stack<Dictionary<GlobalTag, Item>> _globalsStack = new();

        private List<Item> _locals = new();

        private Dictionary<GlobalTag, Item> Globals => _globalsStack.Peek();

        private CollectionItem? CurrentCollection
        {
            get
            {
                if (_collectionDefinitionStack.TryPeek(out var collection))
                    return collection;

                return null;
            }
        }

        public DescriptorReader(byte[] data)
        {
            _data = data;
            _globalsStack.Push(new Dictionary<GlobalTag, Item>());
        }

        public List<CollectionItem> ReadRawDescriptor()
        {
            _topLevelCollections.Clear();

#if DEBUG
            DumpDescriptor(_data);
#endif
            using (var ms = new MemoryStream(_data))
            using (var br = new BinaryReader(ms))
            {
                Parse(br);
            }

            return _topLevelCollections;
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

            Console.WriteLine();
        }

        private void Parse(BinaryReader br)
        {
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var (size, type, tagByte) = ParseItemHeader(br.ReadByte());

                switch (type)
                {
                    case ItemType.Main:
                    {
                        HandleMainItem(br, size, (MainTag)tagByte);
                        break;
                    }

                    case ItemType.Global:
                    {
                        HandleGlobalItem(br, size, (GlobalTag)tagByte);
                        break;
                    }

                    case ItemType.Local:
                    {
                        HandleLocalItem(br, size, (LocalTag)tagByte);
                        break;
                    }

                    case ItemType.Reserved:
                    {
                        throw new FormatException("bType field of 0b11 is reserved.");
                    }

                    default:
                    {
                        throw new FormatException($"bType field of {type} is malformed.");
                    }
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

        private void HandleMainItem(BinaryReader br, byte size, MainTag tag)
        {
            var value = FetchItemValue(br, size);

            switch (tag)
            {
                case MainTag.Collection:
                {
                    EnterCollection((CollectionType)value);
                    break;
                }

                case MainTag.EndCollection:
                {
                    ExitCollection();
                    break;
                }

                case MainTag.Input:
                {
                    if (CurrentCollection == null)
                        throw new FormatException("Attempt to use `Input` item outside of a collection.");

                    AddItemWithCurrentState(new InputItem(size, value));
                    break;
                }

                case MainTag.Output:
                {
                    if (CurrentCollection == null)
                        throw new FormatException("Attempt to use `Output` item outside of a collection.");

                    AddItemWithCurrentState(new OutputItem(size, value));
                    break;
                }

                case MainTag.Feature:
                {
                    if (CurrentCollection == null)
                        throw new FormatException("Attempt to use `Feature` item outside of a collection.");

                    AddItemWithCurrentState(new FeatureItem(size, value));
                    break;
                }
            }
        }

        private void HandleGlobalItem(BinaryReader br, byte size, GlobalTag tag)
        {
            var value = FetchItemValue(br, size);

            switch (tag)
            {
                case GlobalTag.Push:
                {
                    DuplicateGlobals();
                    break;
                }

                case GlobalTag.Pop:
                {
                    _globalsStack.Pop();
                    break;
                }

                default:
                {
                    Globals[tag] = new Item(tag, size, value);
                    break;
                }
            }
        }

        private void HandleLocalItem(BinaryReader br, byte size, LocalTag tag)
        {
            var value = FetchItemValue(br, size);
            var item = new Item(tag, size, value);

            _locals.Add(item);
        }

        private int FetchItemValue(BinaryReader br, byte size)
        {
            var value = 0;
            for (var i = 0; i < size; i++)
            {
                value |= (br.ReadByte() << (8 * i));
            }

            return value;
        }

        private void EnterCollection(CollectionType type)
        {
            var collection = new CollectionItem(type, CurrentCollection);

            foreach (var (_, item) in Globals)
                collection.AddItem(item);

            foreach (var local in _locals)
                collection.AddItem(local);

            if (CurrentCollection != null)
                CurrentCollection.AddItem(collection);

            _collectionDefinitionStack.Push(collection);
            _locals.Clear();
        }

        private void ExitCollection()
        {
            var collection = _collectionDefinitionStack.Pop();

            if (CurrentCollection == null)
                _topLevelCollections.Add(collection);
        }

        private void AddItemWithCurrentState(Item item)
        {
            foreach (var (_, global) in Globals)
                item.AddItem(global);

            foreach (var local in _locals)
                item.AddItem(local);

            CurrentCollection!.AddItem(item);
            _locals.Clear();
        }

        private void DuplicateGlobals()
        {
            var dictionary = new Dictionary<GlobalTag, Item>();

            foreach (var (tag, item) in Globals)
                dictionary.Add(tag, item);

            _globalsStack.Push(dictionary);
        }
    }
}