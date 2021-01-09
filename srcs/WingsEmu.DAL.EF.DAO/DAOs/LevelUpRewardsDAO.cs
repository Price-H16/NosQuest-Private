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
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class LevelUpRewardsDAO : MappingBaseDao<LevelUpRewards, LevelUpRewardsDTO>, ILevelUpRewardsDAO
    {
        public LevelUpRewardsDAO(IMapper mapper) : base(mapper)
        {
        }

        public IEnumerable<LevelUpRewardsDTO> LoadByLevelAndLevelType(byte level, LevelType type)
        {
            switch (type)
            {
                case LevelType.JobLevel:
                    return LoadByJobLevel(level);
                case LevelType.Heroic:
                    return LoadByHeroLevel(level);
                default:
                    return LoadByLevel(level);
            }
        }

        private IEnumerable<LevelUpRewardsDTO> LoadByLevel(byte? level)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LevelUpRewards reward in context.LevelUpRewards.Where(s => s.Level == level))
                {
                    yield return _mapper.Map<LevelUpRewardsDTO>(reward);
                }
            }
        }

        private IEnumerable<LevelUpRewardsDTO> LoadByJobLevel(byte? level)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LevelUpRewards reward in context.LevelUpRewards.Where(s => s.JobLevel == level))
                {
                    yield return _mapper.Map<LevelUpRewardsDTO>(reward);
                }
            }
        }

        private IEnumerable<LevelUpRewardsDTO> LoadByHeroLevel(byte? level)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LevelUpRewards reward in context.LevelUpRewards.Where(s => s.HeroLvl == level))
                {
                    yield return _mapper.Map<LevelUpRewardsDTO>(reward);
                }
            }
        }
    }
}