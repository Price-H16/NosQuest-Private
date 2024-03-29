﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading.Tasks;
using OpenNos.GameObject.Networking;
using Qmmands;
using WingsEmu.Commands.Entities;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Commands.Checks
{
    public sealed class RequireAuthorityAttribute : CheckBaseAttribute
    {
        /// <summary>
        /// This represents the Authority level required to execute a command.
        /// </summary>
        public AuthorityType Authority { get; }

        public RequireAuthorityAttribute(AuthorityType authority)
        {
            Authority = authority;
        }

        /// <inheritdoc />
        /// <summary>
        /// This is a check (pre-condition) before trying to execute a command that needs to pass this check.
        /// </summary>
        /// <param name="context">Context of the command. It needs to be castable to a WingsEmuIngameCommandContext in our case.</param>
        /// <returns></returns>
        public override Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider _)
        {
            if (!(context is WingsEmuIngameCommandContext ctx))
            {
                return Task.FromResult(new CheckResult("Invalid context. This is *very* bad. Please report this."));
            }

            if (ctx.Player is ClientSession player && player.Account.Authority < Authority)
            {
                return Task.FromResult(new CheckResult("You (at least) need to be a " + Authority + " in order to execute that command."));
            }

            return Task.FromResult(CheckResult.Successful);
        }
    }
}