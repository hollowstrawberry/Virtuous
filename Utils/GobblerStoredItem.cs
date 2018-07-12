using Terraria;

namespace Virtuous.Utils
{
    public struct GobblerStoredItem
    {
        public int type;
        public byte prefix;

        public GobblerStoredItem(int type, byte prefix)
        {
            this.type = type;
            this.prefix = prefix;
        }


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
            return item != null && item.Value == this;
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
