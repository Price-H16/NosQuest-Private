// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using Qmmands;

namespace WingsEmu.Commands.Entities
{
    public class SaltyModuleBase : ModuleBase<WingsEmuIngameCommandContext>
    {
        /// <summary>
        ///     This intends to fill the current Context with the command being executed.
        /// </summary>
        protected override Task BeforeExecutedAsync(Command command)
        {
            Context.Command = command;

            return base.BeforeExecutedAsync(command);
        }
    }
}