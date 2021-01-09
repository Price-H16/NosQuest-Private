// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.Enums
{
    public enum AuthorityType : short
    {
        Closed = -3,
        Banned = -2,
        Unconfirmed = -1,
        User = 0,
        Vip = 1,
        VipPlus = 3,
        VipPlusPlus = 5,
        Donator = 10,
        DonatorPlus = 15,
        DonatorPlusPlus = 20,
        Moderator = 25,
        GameMaster = 40,
        SuperGameMaster = 50,
        Administrator = 100
    }
}