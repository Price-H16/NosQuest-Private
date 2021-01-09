// WingsEmu
// 
// Developed by NosWings Team

namespace OpenNos.DAL.EF.Entities
{
    public class MapTypeMap
    {
        #region Properties

        public int Id { get; set; }

        public virtual Map Map { get; set; }

        public short MapId { get; set; }

        public virtual MapType MapType { get; set; }

        public short MapTypeId { get; set; }

        #endregion
    }
}