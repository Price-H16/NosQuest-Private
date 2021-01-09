// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using OpenNos.GameObject.Commands;
using OpenNos.GameObject.Networking;
using WingsEmu.Commands.Interfaces;

namespace WingsEmu.Commands
{
    public class CommandGlobalExecutorWrapper : IGlobalCommandExecutor
    {
        private readonly ICommandContainer _commandContainer;

        public CommandGlobalExecutorWrapper(ICommandContainer commandContainer)
        {
            _commandContainer = commandContainer;
        }

        public void HandleCommand(string command, ClientSession sender)
        {
            HandleCommandAsync(command, sender).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleCommandAsync(string command, ClientSession sender)
        {
            await _commandContainer.HandleMessageAsync(command, sender);
        }
    }
}