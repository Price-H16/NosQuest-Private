// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.Packets
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PacketAliasAttribute : Attribute
    {
        #region Instantiation

        public PacketAliasAttribute(string alias) => Alias = alias;

        #endregion

        #region Properties

        public string Alias { get; set; }

        #endregion
    }
}