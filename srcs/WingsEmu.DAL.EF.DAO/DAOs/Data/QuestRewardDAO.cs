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
    public class QuestRewardDAO : MappingBaseDao<QuestReward, QuestRewardDTO>, IQuestRewardDAO
    {
        public QuestRewardDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<QuestRewardDTO> questRewards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (QuestRewardDTO rewards in questRewards)
                    {
                        var entity = _mapper.Map<QuestReward>(rewards);
                        context.QuestReward.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public QuestRewardDTO Insert(QuestRewardDTO questReward)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<QuestReward>(questReward);
                    context.QuestReward.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<QuestRewardDTO>(questReward);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public List<QuestRewardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.QuestReward.ToList().Select(d => _mapper.Map<QuestRewardDTO>(d)).ToList();
            }
        }

        public IEnumerable<QuestRewardDTO> LoadByQuestId(long questId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuestReward reward in context.QuestReward.Where(s => s.QuestId == questId))
                {
                    yield return _mapper.Map<QuestRewardDTO>(reward);
                }
            }
        }

        #endregion
    }
}