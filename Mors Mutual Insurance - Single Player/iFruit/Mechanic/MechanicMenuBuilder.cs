using GTA;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Teleport;
using MMI_SP.Insurance.Delivery;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using NativeUI;
using System;
using System.Collections.Generic;

namespace MMI_SP.iFruit
{
    internal static class MechanicMenuBuilder
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly Random _rnd = new Random();

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void FillBring(UIMenu submenu, Action onClose)
        {
            submenu.Clear();

            var vehiclesToBring = Bring.GetBringableVehicles(Insurance.Observer.Manager.RecoveredVehList, Insurance.Observer.Manager.InsuredVehList);
            var dormantList = Dormancy.Core.GetDormantList();

            if (vehiclesToBring.Count == Constants.ZERO && dormantList.Count == Constants.ZERO)
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
                        aliveKeys.Add(VehicleKey.FullKeyFrom(vd));
                        return true;
                    },
                    onNone: () => false
                );
            }

            foreach (Vehicle veh in vehiclesToBring)
            {
                if (VehiclesInGarage.IsDefaultGarage(veh)) continue;

                string vehId = VehicleIdentifier.Get(veh);
                int cost = Calculator.GetRecoverCost(vehId);
                float distance = Game.Player.Character.Position.DistanceTo(veh.Position);
                bool tooClose = distance < Constants.TOO_CLOSE_DISTANCE;
                bool isCurrentVehicle = (veh == Game.Player.Character.CurrentVehicle);
                bool enabled = !(tooClose || isCurrentVehicle);

                string label = VehicleIdentifier.GetLocalizedDisplayNameFromId(vehId);
                string description;

                if (isCurrentVehicle)
                {
                    description = Constants.CURRENT_VEHICLE_JOKES[_rnd.Next(Constants.CURRENT_VEHICLE_JOKES.Length)];
                }
                else if (tooClose)
                {
                    description = Constants.TOO_CLOSE_JOKES[_rnd.Next(Constants.TOO_CLOSE_JOKES.Length)];
                }
                else
                {
                    description = $"Coste de envío: ~g~{cost}$~w~";
                }

                MechanicItemFactory.AddVehicleItem(submenu, label, description, enabled, () =>
                {
                    Teleport.BehindPlayer(veh);
                    Insurance.Delivery.Manager.RequestDelivery(veh, cost, false, false, Insurance.Observer.Manager.BlipsToRemove);
                    Notification.ShowMechanic("Mecánico", "Marchando. Voy para allá, no te muevas.");
                    MechanicSound.Play(MechanicSound.SoundFamily.Confirm);
                }, cost, onClose);
            }

            foreach (var dormant in dormantList)
            {
                if (dormant?.Data == null) continue;

                string dormantKey = VehicleKey.FullKeyFrom(dormant.Data);
                if (aliveKeys.Contains(dormantKey)) continue;

                int cost = Calculator.GetRecoverCost(dormant.Data.Id);
                MechanicItemFactory.AddDormantItem(submenu, dormant, cost, onClose);
            }
        }
    }
}