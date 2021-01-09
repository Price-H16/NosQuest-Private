// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;

namespace WingsEmu.DTOs
{
    public class WorldServerDTO
    {
        #region Instantiation

        public WorldServerDTO(Guid id, int accountLimit)
        {
            ConnectedAccounts = new Dictionary<string, long>();
            ConnectedCharacters = new Dictionary<string, long>();
            Id = id;
            AccountLimit = accountLimit;
        }

        #endregion

        #region Properties

        public int AccountLimit { get; set; }

        public int ChannelId { get; set; }

        public Dictionary<string, long> ConnectedAccounts { get; set; }

        public Dictionary<string, long> ConnectedCharacters { get; set; }

        public Guid Id { get; set; }

        #endregion
    }
}