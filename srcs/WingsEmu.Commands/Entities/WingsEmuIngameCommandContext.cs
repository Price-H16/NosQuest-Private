// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Networking;
using Qmmands;

namespace WingsEmu.Commands.Entities
{
    public sealed class WingsEmuIngameCommandContext : ICommandContext
    {
        public CommandService CommandService { get; }

        public Command Command { get; set; }

        public string Message { get; set; }
        public ClientSession Player { get; set; }

        public string Input { get; set; }

        public WingsEmuIngameCommandContext(string message, ClientSession sender, CommandService cmds)
        {
            CommandService = cmds;

            Message = message;
            Player = sender;

            var pos = message.IndexOf('$') + 2;
            Input = message.Substring(pos);
        }
    }
}