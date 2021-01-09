// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Interfaces;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items.Instance
{
    public class BoxInstance : SpecialistInstance, IBoxInstance
    {
        #region Members

        private Random _random;

        #endregion

        #region Instantiation

        public BoxInstance() => _random = new Random();

        public BoxInstance(Guid id)
        {
            Id = id;
            _random = new Random();
        }

        #endregion

        #region Properties

        public short HoldingVNum { get; set; }

        public MateType MateType { get; set; }

        #endregion
    }
}