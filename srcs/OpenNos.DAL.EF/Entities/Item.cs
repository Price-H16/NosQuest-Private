// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    /// <summary>
    ///     Do Not forget to change Mapping in Item GO when changing this class
    /// </summary>
    public class Item
    {
        #region Instantiation

        public Item()
        {
            Drop = new HashSet<Drop>();
            Recipe = new HashSet<Recipe>();
            Mail = new HashSet<Mail>();
            RollGeneratedItem = new HashSet<RollGeneratedItem>();
            RollGeneratedItem2 = new HashSet<RollGeneratedItem>();
            RecipeItem = new HashSet<RecipeItem>();
            ShopItem = new HashSet<ShopItem>();
            BCards = new HashSet<BCard>();
        }

        #endregion

        #region Properties

        public byte BasicUpgrade { get; set; }
        public bool Flag1 { get; set; }
        public bool Flag2 { get; set; }
        public bool Flag3 { get; set; }
        public bool Flag4 { get; set; }
        public bool Flag5 { get; set; }
        public bool Flag6 { get; set; }
        public bool Flag7 { get; set; }
        public bool Flag8 { get; set; }

        public byte CellonLvl { get; set; }

        public byte Class { get; set; }

        public short CloseDefence { get; set; }

        public byte Color { get; set; }

        public short Concentrate { get; set; }

        public byte CriticalLuckRate { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public byte DarkElement { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public virtual ICollection<Drop> Drop { get; set; }

        public short Effect { get; set; }

        public int EffectValue { get; set; }

        public byte Element { get; set; }

        public short ElementRate { get; set; }

        public EquipmentType EquipmentSlot { get; set; }

        public byte FireElement { get; set; }

        public short FireResistance { get; set; }

        public byte Height { get; set; }

        public short HitRate { get; set; }

        public short Hp { get; set; }

        public short HpRegeneration { get; set; }

        public bool IsMinilandActionable { get; set; }

        public bool IsColored { get; set; }

        public bool IsConsumable { get; set; }

        public bool IsDroppable { get; set; }

        public bool IsHeroic { get; set; }

        public bool Flag9 { get; set; }

        public bool IsWarehouse { get; set; }

        public bool IsSoldable { get; set; }

        public bool IsTradable { get; set; }

        public virtual ICollection<BCard> BCards { get; set; }

        public virtual ICollection<ItemInstance> ItemInstances { get; set; }

        public byte ItemSubType { get; set; }

        public byte ItemType { get; set; }

        public long ItemValidTime { get; set; }

        public byte LevelJobMinimum { get; set; }

        public byte LevelMinimum { get; set; }

        public byte LightElement { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public virtual ICollection<Mail> Mail { get; set; }

        public virtual ICollection<RollGeneratedItem> RollGeneratedItem { get; set; }

        public virtual ICollection<RollGeneratedItem> RollGeneratedItem2 { get; set; }

        public byte MaxCellon { get; set; }

        public byte MaxCellonLvl { get; set; }

        public short MaxElementRate { get; set; }

        public byte MaximumAmmo { get; set; }

        public int MinilandObjectPoint { get; set; }

        public short MoreHp { get; set; }

        public short MoreMp { get; set; }

        public short Morph { get; set; }

        public short Mp { get; set; }

        public short MpRegeneration { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public long Price { get; set; }

        public short PvpDefence { get; set; }

        public byte PvpStrength { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }

        public virtual ICollection<RecipeItem> RecipeItem { get; set; }

        public short ReduceOposantResistance { get; set; }

        public byte ReputationMinimum { get; set; }

        public long ReputPrice { get; set; }

        public byte SecondaryElement { get; set; }

        public byte Sex { get; set; }

        public virtual ICollection<ShopItem> ShopItem { get; set; }

        public byte Speed { get; set; }

        public byte SpType { get; set; }

        public byte Type { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short VNum { get; set; }

        public short WaitDelay { get; set; }

        public byte WaterElement { get; set; }

        public short WaterResistance { get; set; }

        public byte Width { get; set; }

        #endregion
    }
}