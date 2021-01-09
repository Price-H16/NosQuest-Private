// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.DTOs.Interfaces
{
    public interface ISpecialistInstance : IWearableInstance
    {
        #region Properties

        short PartnerSkill1 { get; set; }

        short PartnerSkill2 { get; set; }

        short PartnerSkill3 { get; set; }

        byte SkillRank1 { get; set; }

        byte SkillRank2 { get; set; }

        byte SkillRank3 { get; set; }

        short SlDamage { get; set; }

        short SlDefence { get; set; }

        short SlElement { get; set; }

        short SlHP { get; set; }

        byte SpDamage { get; set; }

        byte SpDark { get; set; }

        byte SpDefence { get; set; }

        byte SpElement { get; set; }

        byte SpFire { get; set; }

        byte SpHP { get; set; }

        byte SpLevel { get; set; }

        byte SpLight { get; set; }

        byte SpStoneUpgrade { get; set; }

        byte SpWater { get; set; }

        #endregion
    }
}