// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs;

namespace WingsEmu.Plugins.DB.MSSQL.Mapping
{
    public class WingsEmuItemInstanceMappingType : ItemInstanceDAO.IItemInstanceMappingTypes
    {
        public List<(Type, Type)> Types { get; } = new List<(Type, Type)>
        {
            (typeof(OpenNos.GameObject.Items.Instance.BoxInstance), typeof(BoxInstance)),
            (typeof(OpenNos.GameObject.Items.Instance.ItemInstance), typeof(ItemInstance)),
            (typeof(OpenNos.GameObject.Items.Instance.SpecialistInstance), typeof(SpecialistInstance)),
            (typeof(OpenNos.GameObject.Items.Instance.WearableInstance), typeof(WearableInstance)),
        };
    }
}