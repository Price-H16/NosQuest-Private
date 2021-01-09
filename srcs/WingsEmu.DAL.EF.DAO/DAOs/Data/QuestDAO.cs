﻿// WingsEmu
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
    public class QuestDAO : MappingBaseDao<Quest, QuestDTO>, IQuestDAO
    {
        public QuestDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void InsertOrUpdate(List<QuestDTO> quests)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (QuestDTO q in quests)
                    {
                        if (context.Quest.Any(s => s.InfoId == q.InfoId))
                        {
                            Quest oldQuest = context.Quest.SingleOrDefault(s => s.InfoId == q.InfoId);
                            // Update
                            Update(oldQuest, q, context);
                        }
                        else
                        {
                            //insert
                            Insert(q);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void Insert(List<QuestDTO> quests)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (QuestDTO quest in quests)
                    {
                        var entity = _mapper.Map<Quest>(quest);
                        context.Quest.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public QuestDTO Update(Quest quest, QuestDTO newQuest, OpenNosContext context)
        {
            if (quest != null)
            {
                _mapper.Map(newQuest, quest);
                context.SaveChanges();
            }

            return _mapper.Map<QuestDTO>(quest);
        }

        public QuestDTO Insert(QuestDTO quest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Quest>(quest);
                    context.Quest.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<QuestDTO>(quest);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public List<QuestDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Quest.ToList().Select(d => _mapper.Map<QuestDTO>(d)).ToList();
            }
        }


        public QuestDTO LoadById(long questId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<QuestDTO>(context.Quest.FirstOrDefault(s => s.QuestId.Equals(questId)));
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