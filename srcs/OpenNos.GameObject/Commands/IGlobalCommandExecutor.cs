// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Commands
{
    public interface IGlobalCommandExecutor
    {
        /// <summary>
        ///     Method which will parse the message and try to execute the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="sender"></param>
        void HandleCommand(string command, ClientSession sender);

        /// <summary>
        ///     Method which will parse the message and try to execute the command.
        /// </summary>
        /// <param name="command">Raw message to parse.</param>
        /// <param name="sender"></param>
        Task HandleCommandAsync(string command, ClientSession sender);
    }
}