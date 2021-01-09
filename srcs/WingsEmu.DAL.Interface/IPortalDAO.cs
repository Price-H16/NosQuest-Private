// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IPortalDAO : IMappingBaseDAO
    {
        #region Methods

        PortalDTO Insert(PortalDTO portal);

        void Insert(List<PortalDTO> portals);

        IEnumerable<PortalDTO> LoadByMap(short MapId);

        #endregion

        IEnumerable<PortalDTO> LoadAll();
    }
}