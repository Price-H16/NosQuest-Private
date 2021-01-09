// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface ITeleporterDAO : IMappingBaseDAO
    {
        #region Methods

        TeleporterDTO Insert(TeleporterDTO teleporter);

        IEnumerable<TeleporterDTO> LoadAll();

        TeleporterDTO LoadById(short TeleporterId);

        IEnumerable<TeleporterDTO> LoadFromNpc(int NpcId);

        #endregion
    }
}