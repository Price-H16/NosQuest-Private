// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading.Tasks;
using Qmmands;
using WingsEmu.Commands.Entities;

namespace WingsEmu.Commands.Checks
{
    public sealed class PlayerInMapAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            var ctx = context as WingsEmuIngameCommandContext;

            return ctx.Player.HasCurrentMapInstance is false
                ? Task.FromResult(new CheckResult("You need to be in a map to execute that command."))
                : Task.FromResult(CheckResult.Successful);
        }
    }
}
