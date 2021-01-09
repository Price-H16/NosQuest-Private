// WingsEmu
// 
// Developed by NosWings Team

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenNos.DAL.EF.DB
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OpenNosContext>
    {
        public OpenNosContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OpenNosContext> optionsBuilder = new DbContextOptionsBuilder<OpenNosContext>();

            optionsBuilder.UseSqlServer("Server=localhost,1433;User id=sa;Password=strong_pass2018;Initial Catalog=noswings_game;");
            return new OpenNosContext(optionsBuilder.Options);
        }
    }
}