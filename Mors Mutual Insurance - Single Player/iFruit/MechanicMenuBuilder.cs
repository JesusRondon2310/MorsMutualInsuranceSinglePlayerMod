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
                bool tooClose = distance < 40f;
                bool isCurrentVehicle = (veh == Game.Player.Character.CurrentVehicle);
                bool enabled = !(tooClose || isCurrentVehicle);

                string label = VehicleIdentifier.GetLocalizedDisplayNameFromId(vehId);
                string description;

                if (isCurrentVehicle) {
                    string[] jokes = {
                        "Te reto a presionar el boton. No hay huevos.",
                        "Ese eres tú, payaso.",
                        "No clono autos con la mente, bro. Bájate primero y luego le das al botón.",
                        "Mira abajo, genio. Ya estás en el auto. Dale al botón todo lo que quieras, no hago magia.",
                        "¿Quieres que traiga el coche en el que vas montado?",
                        "¿Y qué estás manejando? Veo que tienes 160 de IQ.",
                        "¿Te lele la cabeshita?",
                        "Espérame ahí, ya voy llegando. ¿Crees que nací ayer o qué?"
                    };
                    description = jokes[_rnd.Next(jokes.Length)];
                }
                else if (tooClose)
                {
                    string[] tooCloseJokes = {
                        "¿En serio? Lo tienes al lado. ¡Usa las piernas!",
                        "¿Bro? Camina dos pasos, no seas tan perezoso.",
                        "No seáis tan flojo y caminá, vago."
                    };
                    description = tooCloseJokes[_rnd.Next(tooCloseJokes.Length)];
                }
                else {
                    description = $"Coste de envío: ~g~{cost}$~w~";
                }

                MechanicItemFactory.AddVehicleItem(submenu, label, description, enabled, () =>
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

                string dormantKey = VehicleKey.FullKeyFrom(dormant.Data);
                if (aliveKeys.Contains(dormantKey)) continue;

                int cost = Calculator.GetRecoverCost(dormant.Data.Id);
                MechanicItemFactory.AddDormantItem(submenu, dormant, cost, onClose);
            }
        }
    }
}