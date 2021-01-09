// WingsEmu
// 
// Developed by NosWings Team

using System.ComponentModel.DataAnnotations.Schema;

namespace OpenNos.DAL.EF.Entities
{
    public class MapMonster
    {
        #region Properties

        public bool IsDisabled { get; set; }

        public bool IsMoving { get; set; }

        public virtual Map Map { get; set; }

        public short MapId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MapMonsterId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public short MonsterVNum { get; set; }

        public virtual NpcMonster NpcMonster { get; set; }

        public byte Position { get; set; }

        #endregion
    }
}