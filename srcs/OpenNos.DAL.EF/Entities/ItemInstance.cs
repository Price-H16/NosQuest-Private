// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OpenNos.DAL.EF.Entities.Base;

namespace OpenNos.DAL.EF.Entities
{
    public class ItemInstance : SynchronizableBaseEntity
    {
        #region Instantiation

        public ItemInstance()
        {
            BazaarItem = new HashSet<BazaarItem>();
            MinilandObject = new HashSet<MinilandObject>();
        }

        #endregion

        #region Properties

        public int Amount { get; set; }

        public virtual ICollection<BazaarItem> BazaarItem { get; set; }

        public long? BazaarItemId { get; set; }

        [ForeignKey(nameof(BoundCharacterId))]
        public Character BoundCharacter { get; set; }

        public long? BoundCharacterId { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public short Design { get; set; }

        public int DurabilityPoint { get; set; }

        public virtual Item Item { get; set; }

        public DateTime? ItemDeleteTime { get; set; }

        public short ItemVNum { get; set; }

        public virtual ICollection<MinilandObject> MinilandObject { get; set; }

        public short Rare { get; set; }

        public short Slot { get; set; }

        public byte Type { get; set; }

        public byte Upgrade { get; set; }

        //Partner agility
        public byte Agility { get; set; }

        public short PartnerSkill1 { get; set; }

        public short PartnerSkill2 { get; set; }

        public short PartnerSkill3 { get; set; }

        public byte SkillRank1 { get; set; }

        public byte SkillRank2 { get; set; }

        public byte SkillRank3 { get; set; }

        #endregion
    }
}