// WingsEmu
// 
// Developed by NosWings Team

namespace OpenNos.GameObject.Battle.Args
{
    public class HitArgs
    {
        public IBattleEntity HitSource { get; set; }
        public uint Damage { get; set; }
        public Skill Skill { get; set; }
    }
}