using GTA;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Teleport;
using MMI_SP.Insurance.Delivery;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using NativeUI;
using System.Collections.Generic;

namespace MMI_SP.iFruit
{
    internal static class MechanicMenuBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void FillBring(UIMenu submenu, System.Action onClose)
        {
            submenu.Clear();

            var vehiclesToBring = Bringable.Execute(Insurance.Observer.Manager.RecoveredVehList, Insurance.Observer.Manager.InsuredVehList);
            var dormantList = Dormancy.Core.GetDormantList();

            if (vehiclesToBring.Count == 0 && dormantList.Count == 0)
            {
                var empty = new UIMenuItem("Vacío", "No tienes vehículos disponibles.");
                empty.Enabled = false;
                submenu.AddItem(empty);
                return;
            }

            var aliveKeys = new HashSet<string>();
            foreach (var v in vehiclesToBring)
            {
                string vehId = VehicleIdentifier.Get(v);
                var dataOption = DB.Core.FindVehicle(vehId);
                dataOption.match<bool>(
                    onSome: vd =>
                    {
                        aliveKeys.Add(VehicleKey.From(vd));
                        return true;
                    },
                    onNone: () => false
                );
            }

            foreach (Vehicle veh in vehiclesToBring)
            {
                if (VehiclesInGarage.IsInAnyGarage(veh)) continue;

                string vehId = VehicleIdentifier.Get(veh);
                int cost = Calculator.GetRecoverCost(vehId);
                float distance = Game.Player.Character.Position.DistanceTo(veh.Position);
                bool tooClose = distance < 40f;

                string label = VehicleIdentifier.GetLocalizedDisplayNameFromId(vehId);
                string description = tooClose
                    ? "¿En serio? Lo tienes al lado. Usa las piernas."
                    : $"Coste de envío: ~g~{cost}$~w~";

                MechanicItemFactory.AddVehicleItem(submenu, label, description, !tooClose, () =>
                {
                    BehindPlayer.Execute(veh);
                    Insurance.Delivery.Manager.RequestDelivery(veh, cost, false, false,
                        Insurance.Observer.Manager.RecoveredVehList, Insurance.Observer.Manager.BlipsToRemove);
                    Notification.ShowMechanic("Mecánico", "Marchando. Voy para allá, no te muevas.");
                    MechanicSound.Play(MechanicSound.SoundFamily.Confirm);
                }, cost, onClose);
            }

            foreach (var dormant in dormantList)
            {
                if (dormant?.Data == null) continue;

                string dormantKey = $"{dormant.Data.ModelName}_{dormant.Data.Plate}";
                if (aliveKeys.Contains(dormantKey)) continue;

                int cost = Calculator.GetRecoverCost(dormant.Data.Id);
                MechanicItemFactory.AddDormantItem(submenu, dormant, cost, onClose);
            }
        }
    }
}