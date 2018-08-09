using Terraria;

namespace Virtuous.Utils
{
    /// <summary>Stores an item as its basic type and prefix in order to build it again in the future.</summary>
    public struct GobblerStoredItem
    {
        /// <summary>The type of the stored item.</summary>
        public int type;

        /// <summary>The prefix of the stored item.</summary>
        public byte prefix;


        /// <summary>Creates a <see cref="GobblerStoredItem"/> from an item's type and prefix.</summary>
        public GobblerStoredItem(int type, byte prefix)
        {
            this.type = type;
            this.prefix = prefix;
        }

        /// <summary>Creates a <see cref="GobblerStoredItem"/> from the given item.</summary>
        public GobblerStoredItem(Item item)
        {
            type = item.type;
            prefix = item.prefix;
        }


        /// <summary>Builds an instance of the stored item.</summary>
        public Item MakeItem()
        {
            var item = new Item();
            item.SetDefaults(type);
            item.prefix = prefix;
            return item;
        }


        public override bool Equals(object obj)
        {
            var item = obj as GobblerStoredItem?;
            return item.HasValue && item.Value == this;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ prefix.GetHashCode();
        }


        public static bool operator ==(GobblerStoredItem item1, GobblerStoredItem item2)
        {
            return item1.type == item2.type && item1.prefix == item2.prefix;
        }

        public static bool operator !=(GobblerStoredItem item1, GobblerStoredItem item2) => !(item1 == item2);
    }
}
