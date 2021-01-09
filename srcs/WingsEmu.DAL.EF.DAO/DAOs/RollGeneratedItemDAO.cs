// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class RollGeneratedItemDAO : MappingBaseDao<RollGeneratedItem, RollGeneratedItemDTO>, IRollGeneratedItemDAO
    {
        public RollGeneratedItemDAO(IMapper mapper) : base(mapper)
        {
            IEnumerable<RollGeneratedItemDTO> bcards = LoadAll();


            _rollItems = bcards.GroupBy(s => s.OriginalItemVNum).ToDictionary(s => s.Key, s => s.ToArray());
        }

        #region Methods

        private readonly Dictionary<short, RollGeneratedItemDTO[]> _rollItems;

        public IEnumerable<RollGeneratedItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.RollGeneratedItem.ToArray().Select(_mapper.Map<RollGeneratedItemDTO>);
            }
        }

        public IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum)
        {
            if (!_rollItems.TryGetValue(vnum, out RollGeneratedItemDTO[] rollItems))
            {
                return null;
            }

            return rollItems.Select(_mapper.Map<RollGeneratedItemDTO>);
        }

        #endregion
    }
}