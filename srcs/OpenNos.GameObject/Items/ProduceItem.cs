// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Items
{
    public class ProduceItem : Item
    {
        #region Instantiation

        public ProduceItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            switch (Effect)
            {
                case 100:
                    session.Character.LastNRunId = 0;
                    session.Character.LastUsedItem = VNum;
                    session.SendPacket("wopen 28 0");
                    List<Recipe> recipeList = ServerManager.Instance.GetRecipesByItemVNum(VNum);
                    string list = recipeList.Where(s => s.Amount > 0)
                        .Aggregate("m_list 2", (current, s) => current + $" {s.ItemVNum}");
                    session.SendPacket(list + (EffectValue <= 110 && EffectValue >= 108 ? " 999" : string.Empty));
                    break;
                default:
                    Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}