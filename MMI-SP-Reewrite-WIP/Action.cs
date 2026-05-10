using GTA;
using MMI_SP.Helpers;
using NativeUI;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.Menu
{
    internal static class Buttons
    {
        private const string NotifyChar = "CHAR_CARSITE";
        private const string NotifyTitle = "Mors Mutual";
        private const string NoMoneyMsg = "No tienes suficiente dinero.";

        public static void Insure(UIMenu sender, UIMenuItem selectedItem)
        {
            Vehicle lastVeh = Game.Player.LastVehicle;
            if (lastVeh == null || !lastVeh.Exists()) return;

            if (Core.IsInsured(lastVeh)) return;
            if (!Core.IsInsurable(lastVeh)) return;

            int cost = Core.GetCost(lastVeh);

            if (Game.Player.Money < cost)
            {
                Notification.Show(NotifyChar, NotifyTitle, NoMoneyMsg, "");
                return;
            }

            Game.Player.Money -= cost;
            Core.Insure(lastVeh);
            Notification.Show(NotifyChar, NotifyTitle, "Vehículo asegurado correctamente.", "");

            Execute.Rebuild(sender);
        }
    }
}