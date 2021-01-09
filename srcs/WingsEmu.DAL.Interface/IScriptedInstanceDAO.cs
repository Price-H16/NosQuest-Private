// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IScriptedInstanceDAO : IMappingBaseDAO
    {
        #region Methods

        ScriptedInstanceDTO Insert(ScriptedInstanceDTO portal);

        void Insert(List<ScriptedInstanceDTO> portals);

        IEnumerable<ScriptedInstanceDTO> LoadByMap(short mapId);

        #endregion
    }
}