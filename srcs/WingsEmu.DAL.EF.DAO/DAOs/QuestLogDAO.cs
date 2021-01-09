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
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class QuestLogDAO : MappingBaseDao<QuestLog, QuestLogDTO>, IQuestLogDAO
    {
        public QuestLogDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref QuestLogDTO quest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long questId = quest.QuestId;
                    QuestLog entity = context.QuestLog.FirstOrDefault(c => c.QuestId.Equals(questId));

                    if (entity == null)
                    {
                        quest = Insert(quest, context);
                        return SaveResult.Inserted;
                    }

                    quest.QuestId = entity.QuestId;
                    quest = Update(entity, quest, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public QuestLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<QuestLogDTO>(context.QuestLog.FirstOrDefault(i => i.QuestId == id));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public IEnumerable<QuestLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuestLog id in context.QuestLog.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<QuestLogDTO>(id);
                }
            }
        }

        public SaveResult InsertOrUpdateList(ref List<QuestLogDTO> questList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (QuestLogDTO q in questList)
                    {
                        QuestLogDTO quest = q;
                        long questId = quest.QuestId;
                        QuestLog entity = context.QuestLog.FirstOrDefault(c => c.QuestId.Equals(questId));

                        if (entity == null)
                        {
                            quest = Insert(quest, context);
                            return SaveResult.Inserted;
                        }

                        quest.QuestId = entity.QuestId;
                        quest = Update(entity, quest, context);
                        return SaveResult.Updated;
                    }

                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public QuestLogDTO Insert(QuestLogDTO quest, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<QuestLog>(quest);
                context.QuestLog.Add(entity);
                context.SaveChanges();
                return _mapper.Map<QuestLogDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public QuestLogDTO Update(QuestLog old, QuestLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                _mapper.Map(old, replace);
                context.SaveChanges();
            }

            return _mapper.Map<QuestLogDTO>(old);
        }
    }
}