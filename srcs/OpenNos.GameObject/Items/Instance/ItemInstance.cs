// WingsEmu
// 
// Developed by NosWings Team

using System;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items.Instance
{
    public class ItemInstance : ItemInstanceDTO
    {
        #region Members

        private Random _random;
        private Item _item;

        #endregion

        #region Instantiation

        public ItemInstance() => _random = new Random();

        public ItemInstance(short vNum, ushort amount)
        {
            ItemVNum = vNum;
            Amount = amount;
            Type = Item.Type;
            _random = new Random();
        }

        #endregion

        #region Properties

        public bool IsBound => BoundCharacterId.HasValue && Item.ItemType != ItemType.Armor && Item.ItemType != ItemType.Weapon;

        public Item Item => _item ?? (_item = ServerManager.Instance.GetItem(ItemVNum));

        public ClientSession CharacterSession => ServerManager.Instance.GetSessionByCharacterId(CharacterId);

        #endregion

        //// TODO: create Interface

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.0";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{(this as SpecialistInstance)?.SpStoneUpgrade}";

                case InventoryType.Costume:
                    return $"ivn 7 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.0";
            }

            return string.Empty;
        }

        public string GeneratePStash() => $"pstash {GenerateStashPacket()}";

        public string GenerateStash() => $"stash {GenerateStashPacket()}";

        public string GenerateStashPacket()
        {
            string packet = $"{Slot}.{ItemVNum}.{(byte)Item.Type}";
            switch (Item.Type)
            {
                case InventoryType.Equipment:
                    return packet + $".{Amount}.{Rare}.{Upgrade}";

                case InventoryType.Specialist:
                    var sp = this as SpecialistInstance;
                    return packet + $".{Upgrade}.{sp?.SpStoneUpgrade ?? 0}.0";

                default:
                    return packet + $".{Amount}.0.0";
            }
        }

        public void Save()
        {
        }

        #endregion
    }
}