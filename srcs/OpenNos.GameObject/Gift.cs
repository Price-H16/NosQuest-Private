// WingsEmu
// 
// Developed by NosWings Team

namespace OpenNos.GameObject
{
    public class Gift
    {
        #region Instantiation

        public Gift()
        {
            // do nothing
        }

        public Gift(short vnum, byte amount, short design = 0, bool isRareRandom = true, bool isHeroic = false)
        {
            VNum = vnum;
            Amount = amount;
            IsRandomRare = isRareRandom;
            Design = design;
            IsHeroic = isHeroic;
        }

        #endregion

        #region Properties

        public byte Amount { get; set; }

        public short Design { get; set; }

        public short VNum { get; set; }

        public bool IsRandomRare { get; set; }

        public bool IsHeroic { get; set; }

        #endregion
    }
}