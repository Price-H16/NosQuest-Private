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
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class MailDAO : MappingBaseDao<Mail, MailDTO>, IMailDAO
    {
        public MailDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult DeleteById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Mail mail = context.Mail.First(i => i.MailId.Equals(mailId));

                    if (mail == null)
                    {
                        return DeleteResult.Deleted;
                    }

                    context.Mail.Remove(mail);
                    context.SaveChanges();

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MailDTO mail)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long mailId = mail.MailId;
                    Mail entity = context.Mail.FirstOrDefault(c => c.MailId.Equals(mailId));

                    if (entity == null)
                    {
                        mail = Insert(mail, context);
                        return SaveResult.Inserted;
                    }

                    mail.MailId = entity.MailId;
                    mail = Update(entity, mail, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MailDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mail mail in context.Mail.Where(s => !s.IsSenderCopy && s.ReceiverId == characterId || s.IsSenderCopy && s.SenderId == characterId))
                {
                    yield return _mapper.Map<MailDTO>(mail);
                }
            }
        }

        public IEnumerable<MailDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mail mail in context.Mail)
                {
                    yield return _mapper.Map<MailDTO>(mail);
                }
            }
        }

        public MailDTO LoadById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MailDTO>(context.Mail.FirstOrDefault(i => i.MailId.Equals(mailId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private MailDTO Insert(MailDTO mail, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<Mail>(mail);
                context.Mail.Add(entity);
                context.SaveChanges();
                return _mapper.Map<MailDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private MailDTO Update(Mail entity, MailDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MailDTO>(entity);
        }

        #endregion
    }
}