// WingsEmu
// 
// Developed by NosWings Team

using Microsoft.EntityFrameworkCore;

namespace WingsEmu.DAL.EF.DAO
{
    public interface IContextFactory
    {
        /// <summary>
        ///     Instantiates a new context
        /// </summary>
        /// <returns></returns>
        DbContext NewContext();
    }
}