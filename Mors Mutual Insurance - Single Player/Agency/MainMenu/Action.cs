using GTA;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using MMI_SP.Dialogue;
using MMI_SP.Agency.Office.Ambient;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Action
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void OnActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (selectedItem == null || !selectedItem.Enabled) return;

            Vehicle veh = Game.Player.LastVehicle;
            var result = Manager.Insure(veh);

            result.match<bool>(
                onOk: _ =>
                {
                    string vehName = VehicleIdentifier.GetLocalizedDisplayName(veh);
                    Notification.ShowMMI("Información", $"Tu ~b~{vehName}~w~ está ahora cubierto por Mors Mutual Insurance.");
                    Core.PlayRandom(Core.SpeechType.OfficeSomething, NpcHandler.CurrentNpc);
                    selectedItem.Enabled = false;
                    selectedItem.SetRightLabel("");
                    Cancel.Refresh();
                    return true;
                },
                onErr: error =>
                {
                    Notification.ShowMMI("~r~Error~w~ al asegurar", error);
                    Core.PlayRandom(Core.SpeechType.OfficeSomething, NpcHandler.CurrentNpc);
                    return false;
                }
            );
        }
    }
}