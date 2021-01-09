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
    public class CardDAO : MappingBaseDao<Card, CardDTO>, ICardDAO
    {
        public CardDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<CardDTO> cards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (CardDTO card in cards)
                    {
                        var entity = _mapper.Map<Card>(card);
                        context.Card.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<CardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Card card in context.Card)
                {
                    yield return _mapper.Map<CardDTO>(card);
                }
            }
        }

        public CardDTO LoadById(short cardId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CardDTO>(context.Card.FirstOrDefault(s => s.CardId.Equals(cardId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}