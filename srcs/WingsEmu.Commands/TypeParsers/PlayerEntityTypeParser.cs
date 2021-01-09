// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading.Tasks;
using OpenNos.GameObject.Networking;
using Qmmands;

namespace WingsEmu.Commands.TypeParsers
{
    public sealed class PlayerEntityTypeParser : TypeParser<ClientSession>
    {
        private readonly ServerManager _manager;

        public PlayerEntityTypeParser(ServerManager manager)
        {
            _manager = manager;
        }

        public override Task<TypeParserResult<ClientSession>> ParseAsync(Parameter param, string value, ICommandContext context, IServiceProvider provider)
        {
            ClientSession player = null;

            player = (provider.GetService(typeof(ServerManager)) as ServerManager)?.GetSessionByCharacterName(value);
            player = _manager.GetSessionByCharacterName(value);

            return player is null
                ? Task.FromResult(new TypeParserResult<ClientSession>($"Player {value} is not connected or doesn't exist."))
                : Task.FromResult(new TypeParserResult<ClientSession>(player));
        }
    }
}