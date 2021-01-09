// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class AccountDAO : MappingBaseDao<Account, AccountDTO>, IAccountDAO
    {
        public AccountDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (account != null)
                    {
                        context.Account.Remove(account);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ACCOUNT_ERROR"), accountId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public long GetBankRanking(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.First(c => c.AccountId.Equals(accountId));
                    return context.Account.Where(s => s.BankMoney > account.BankMoney).OrderByDescending(s => s.BankMoney).Count() + 1;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("ERROR_GETTING_RANK"), accountId, e.Message), e);
                return 0;
            }
        }

        public SaveResult InsertOrUpdate(ref AccountDTO account)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long accountId = account.AccountId;
                    Account entity = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (entity == null)
                    {
                        account = Insert(account, context);
                        return SaveResult.Inserted;
                    }

                    account = Update(entity, account, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ACCOUNT_ERROR"), account.AccountId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public bool ContainsAccounts()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Account.FirstOrDefault() != null)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public AccountDTO LoadById(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.AccountId.Equals(accountId));
                    if (account != null)
                    {
                        return _mapper.Map<AccountDTO>(account);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public AccountDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.Name.Equals(name));
                    if (account != null)
                    {
                        return _mapper.Map<AccountDTO>(account);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public void WriteGeneralLog(long accountId, string ipAddress, long? characterId, GeneralLogType logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var log = new GeneralLog
                    {
                        AccountId = accountId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType.ToString(),
                        LogData = logData,
                        CharacterId = characterId
                    };

                    context.GeneralLog.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private AccountDTO Insert(AccountDTO account, OpenNosContext context)
        {
            var entity = _mapper.Map<Account>(account);
            context.Account.Add(entity);
            context.SaveChanges();
            return _mapper.Map<AccountDTO>(entity);
        }

        private AccountDTO Update(Account entity, AccountDTO account, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            // The _mapper breaks context.SaveChanges(), so we need to "map" the data by hand...
            // entity = _mapper.Map<Account>(account);
            entity.Authority = account.Authority;
            entity.Name = account.Name;
            entity.Password = account.Password;
            entity.BankMoney = account.BankMoney;
            entity.Money = account.Money;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return _mapper.Map<AccountDTO>(entity);
        }

        #endregion
    }
}