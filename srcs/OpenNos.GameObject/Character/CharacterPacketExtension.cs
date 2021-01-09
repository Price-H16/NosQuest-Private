// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Linq;
using OpenNos.Core;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Character
{
    public static class CharacterPacketExtension
    {
        public static string GetFamilyRankName(this Character character)
        {
            return Language.Instance.GetMessageFromKey(character.FamilyCharacter.Authority.ToString().ToUpper());
        }

        public static string GenerateGidx(this Character character)
        {
            return character.Family != null
                ? $"gidx 1 {character.CharacterId} {character.Family.FamilyId} {character.Family.Name}({character.GetFamilyRankName()}) {character.Family.FamilyLevel} 0|0|0"
                : $"gidx 1 {character.CharacterId} -1 - 0";
        }

        public static bool IsUsingFairyBooster(this Character character)
        {
            return character.Buff.Any(s => s.Card.CardId == 131);
        }

        public static string GenerateIn(this Character character, bool foe = false)
        {
            // string chars = "!§$%&/()=?*+~#";
            string name = character.Name;
            if (foe)
            {
                name = "!§$%&/()=?*+~#";
            }

            int faction = 0;
            if (ServerManager.Instance.ChannelId == 51)
            {
                faction = (byte)character.Faction + 2;
            }

            int color = character.HairStyle == HairStyleType.Hair8 ? 0 : (byte)character.HairColor;
            ItemInstance fairy = null;
            if (character.Inventory != null)
            {
                ItemInstance headWearable = character.Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                if (headWearable?.Item.IsColored == true)
                {
                    color = headWearable.Design;
                }

                fairy = character.Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
            }

            var shouldChangeMorph = character.IsUsingFairyBooster() && fairy?.Item.Morph > 4 && fairy.Item.Morph != 9 && fairy.Item.Morph != 14;
            return "in 1 " +
                $"{(character.Authority == AuthorityType.Moderator && !character.Undercover ? "[Support]" + name : name)} " +
                "- " +
                $"{character.CharacterId} " +
                $"{character.PositionX} " +
                $"{character.PositionY} " +
                $"{character.Direction} " +
                $"{(character.Undercover ? (byte)AuthorityType.User : character.Authority < AuthorityType.User ? (byte)AuthorityType.User : character.Authority < AuthorityType.GameMaster ? 0 : 2)} " +
                $"{(byte)character.Gender} " +
                $"{(byte)character.HairStyle} " +
                $"{color} " +
                $"{(byte)character.Class} " +
                $"{character.GenerateEqListForPacket()} " +
                $"{Math.Ceiling(character.Hp / character.HpLoad() * 100)} " +
                $"{Math.Ceiling(character.Mp / character.MpLoad() * 100)} " +
                $"{(character.IsSitting ? 1 : 0)} " +
                $"{(character.Group?.GroupType == GroupType.Group ? (long)character.Group?.GroupId : -1)} " +
                $"{(fairy != null && !character.Undercover ? 4 : 0)} " +
                $"{fairy?.Item.Element ?? 0} " +
                "0 " +
                $"{fairy?.Item.Morph + (shouldChangeMorph ? 5 : 0) ?? 0}" +
                " 0 " +
                $"{(character.UseSp || character.IsVehicled ? character.Morph : 0)} " +
                $"{character.GenerateEqRareUpgradeForPacket()} " +
                $"{(!character.Undercover ? foe ? -1 : character.Family?.FamilyId ?? -1 : -1)} " +
                $"{(!character.Undercover ? foe ? name : character.Family?.Name ?? "-" : "-")} " +
                $"{(character.GetDignityIco() == 1 ? character.GetReputIco() : -character.GetDignityIco())} " +
                $"{(character.Invisible ? 1 : 0)} " +
                $"{(character.UseSp ? character.MorphUpgrade : 0)} " +
                $"{faction} " +
                $"{(character.UseSp ? character.MorphUpgrade2 : 0)} " +
                $"{character.Level} " +
                $"{character.Family?.FamilyLevel ?? 0} " +
                $"0|0|0 " +
                $"{character.ArenaWinner} " +
                $"{(character.Authority == AuthorityType.Moderator && !character.Undercover ? 500 : character.Compliment)} " +
                $"{character.Size} " +
                $"{character.HeroLevel}";
        }
    }
}