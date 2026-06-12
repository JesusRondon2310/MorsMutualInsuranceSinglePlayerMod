using System.Collections.Generic;
using GTA;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;

namespace MMI_SP.iFruit.MMI
{
    internal static class MMIMenuBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void FillRecover(UIMenu submenu, System.Action onRefresh)
        {
            submenu.Clear();
            List<string> ids = Manager.GetDestroyedList();

            if (ids.Count == Constants.NONE)
            {
                var empty = new UIMenuItem("Vacío", "No tienes vehículos destruidos.");
                empty.Enabled = false;
                submenu.AddItem(empty);
                return;
            }

            foreach (string id in ids)
            {
                string label = VehicleIdentifier.GetLocalizedDisplayNameFromId(id);
                int cost = Calculator.GetRecoverCost(id);
                var item = new UIMenuItem(label, $"Coste de recuperación: ~g~{cost}$~w~");
                submenu.AddItem(item);

                string capturedId = id;
                int capturedCost = cost;

                item.Activated += (sender, selectedItem) =>
                {
                    if (Game.Player.Money < capturedCost)
                    {
                        Notification.ShowMMI("Información", "No tienes suficiente dinero para asegurar el vehículo.");
                        MMISound.Play(MMISound.SoundFamily.NoMoney);
                        return;
                    }

                    Game.Player.Money -= capturedCost;
                    Manager.RecoverVehicle(capturedId).match<bool>(
                        onOk: _ =>
                        {
                            string vehName = VehicleIdentifier.GetLocalizedDisplayNameFromId(capturedId);
                            Notification.ShowMMI("Información", $"Reclamación aprobada. Tu ~b~{vehName}~w~ está esperando por ti en el depósito.");
                            MMISound.Play(MMISound.SoundFamily.Okay);
                            onRefresh?.Invoke();
                            return true;
                        },
                        onErr: error =>
                        {
                            Game.Player.Money += capturedCost;
                            Notification.ShowMMI("~r~ERROR~w~", error);
                            return false;
                        }
                    );
                };
            }
        }
    }
}