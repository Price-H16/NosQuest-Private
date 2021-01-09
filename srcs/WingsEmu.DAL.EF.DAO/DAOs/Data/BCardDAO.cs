// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class BCardDAO : MappingBaseDao<BCard, BCardDTO>, IBCardDAO
    {
        private readonly Dictionary<short, BCardDTO[]> _bcardsByCardId;
        private readonly Dictionary<short, BCardDTO[]> _bcardsByItemVnum;
        private readonly Dictionary<short, BCardDTO[]> _bcardsByNpcMonsterVnum;
        private readonly Dictionary<short, BCardDTO[]> _bcardsBySkillVnum;

        public BCardDAO(IMapper mapper) : base(mapper)
        {
                IEnumerable<BCardDTO> bcards = LoadAll();


                _bcardsByItemVnum = bcards.Where(s => s.ItemVNum != null).GroupBy(s => s.ItemVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
                _bcardsByCardId = bcards.Where(s => s.CardId != null).GroupBy(s => s.CardId.Value).ToDictionary(s => s.Key, s => s.ToArray());
                _bcardsByNpcMonsterVnum = bcards.Where(s => s.NpcMonsterVNum != null).GroupBy(s => s.NpcMonsterVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
                _bcardsBySkillVnum = bcards.Where(s => s.SkillVNum != null).GroupBy(s => s.SkillVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
        }

        #region Methods

        public void Insert(List<BCardDTO> cards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (BCardDTO card in cards)
                    {
                        var entity = _mapper.Map<BCard>(card);
                        context.BCard.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }


        public IEnumerable<BCardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.BCard.ToArray().Select(_mapper.Map<BCardDTO>);
            }
        }

        public IEnumerable<BCardDTO> LoadByCardId(short cardId)
        {
            if (!_bcardsByCardId.TryGetValue(cardId, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        public IEnumerable<BCardDTO> LoadByItemVNum(short Vnum)
        {
            if (!_bcardsByItemVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        public IEnumerable<BCardDTO> LoadBySkillVNum(short Vnum)
        {
            if (!_bcardsBySkillVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        public void Clean()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (BCard entity in context.BCard)
                {
                    context.BCard.Remove(entity);
                }

                context.SaveChanges();
            }
        }

        public IEnumerable<BCardDTO> LoadByNpcMonsterVNum(short Vnum)
        {
            if (!_bcardsByNpcMonsterVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        #endregion
    }
}