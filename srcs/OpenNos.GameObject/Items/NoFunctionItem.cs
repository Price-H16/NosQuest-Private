// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class NoFunctionItem : Item
    {
        #region Instantiation

        public NoFunctionItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            switch (Effect)
            {
                case 10:
                    const short gillionVNum = 1013;
                    const short cellaVNum = 1014;
                    short[] cristalItems = { 1028, 1029, 1031, 1032, 1033, 1034 };
                    short[] cellonItems = { 1017, 1018, 1019 };
                    short[] soulGemItems = { 1015, 1016 };

                    int extraItems = ServerManager.Instance.RandomNumber(0, 101);

                    if (session.Character.Inventory.CountItem(gillionVNum) <= 0)
                    {
                        // No Gillion                   
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_GILLION"), 11));
                        return;
                    }

                    session.Character.GiftAdd(cellaVNum, (byte)ServerManager.Instance.RandomNumber(5, 11));
                    if (extraItems > 70)
                    {
                        switch ((RefinerType)EffectValue)
                        {
                            case RefinerType.SoulGem:
                                session.Character.GiftAdd(
                                    soulGemItems[ServerManager.Instance.RandomNumber(0, soulGemItems.Length)], 1);
                                break;
                            case RefinerType.Cellon:
                                session.Character.GiftAdd(
                                    cellonItems[ServerManager.Instance.RandomNumber(0, cellonItems.Length)], 1);
                                break;
                            case RefinerType.Crystal:
                                session.Character.GiftAdd(
                                    cristalItems[ServerManager.Instance.RandomNumber(0, cristalItems.Length)], 1);
                                break;
                        }
                    }

                    session.Character.Inventory.RemoveItemAmount(gillionVNum);
                    session.Character.Inventory.RemoveItemAmount(VNum);
                    break;
                default:
                    Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}