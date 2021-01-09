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
    public class QuestObjectiveDAO : MappingBaseDao<QuestObjective, QuestObjectiveDTO>, IQuestObjectiveDAO
    {
        public QuestObjectiveDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<QuestObjectiveDTO> quests)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (QuestObjectiveDTO quest in quests)
                    {
                        var entity = _mapper.Map<QuestObjective>(quest);
                        context.QuestObjective.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public QuestObjectiveDTO Insert(QuestObjectiveDTO quest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<QuestObjective>(quest);
                    context.QuestObjective.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<QuestObjectiveDTO>(quest);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public List<QuestObjectiveDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.QuestObjective.ToList().Select(d => _mapper.Map<QuestObjectiveDTO>(d)).ToList();
            }
        }

        public IEnumerable<QuestObjectiveDTO> LoadByQuestId(long questId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuestObjective reward in context.QuestObjective.Where(s => s.QuestId == questId))
                {
                    yield return _mapper.Map<QuestObjectiveDTO>(reward);
                }
            }
        }

        #endregion
    }
}