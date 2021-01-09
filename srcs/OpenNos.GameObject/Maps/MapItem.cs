﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Maps
{
    public abstract class MapItem
    {
        #region Instantiation

        public MapItem(short x, short y)
        {
            PositionX = x;
            PositionY = y;
            CreatedDate = DateTime.Now;
            TransportId = 0;
        }

        #endregion

        #region Members

        protected ItemInstance ItemInstance;
        private long _transportId;

        #endregion

        #region Properties

        public abstract ushort Amount { get; set; }

        public DateTime CreatedDate { get; set; }

        public abstract short ItemVNum { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public List<EquipmentOptionDTO> EquipmentOptions;

        public virtual long TransportId
        {
            get
            {
                if (_transportId == 0)
                {
                    // create transportId thru factory
                    // TODO: Review has some problems, aka. issue corresponding to weird/multiple/missplaced drops
                    _transportId = TransportFactory.Instance.GenerateTransportId();
                }

                return _transportId;
            }

            set
            {
                if (value != _transportId)
                {
                    _transportId = value;
                }
            }
        }

        #endregion

        #region Methods

        public string GenerateIn() =>
            $"in 9 {ItemVNum} {TransportId} {PositionX} {PositionY} {(this is MonsterMapItem && ((MonsterMapItem)this).GoldAmount > 1 ? ((MonsterMapItem)this).GoldAmount : Amount)} 0 0 -1";

        public string GenerateOut(long id) => $"out 9 {id}";

        public abstract ItemInstance GetItemInstance();

        #endregion
    }
}