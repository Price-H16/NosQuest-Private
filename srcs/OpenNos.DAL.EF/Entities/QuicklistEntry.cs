// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.DAL.EF.Entities.Base;

namespace OpenNos.DAL.EF.Entities
{
    public class QuicklistEntry : SynchronizableBaseEntity
    {
        #region Properties

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public short Morph { get; set; }

        public short Pos { get; set; }

        public short Q1 { get; set; }

        public short Q2 { get; set; }

        public short Slot { get; set; }

        public short Type { get; set; }

        #endregion
    }
}