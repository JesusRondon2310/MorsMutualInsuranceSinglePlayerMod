using GTA;
using MMI_SP.Dormancy;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Teleport;
using MMI_SP.PatternMatching;
using NativeUI;
using System;

namespace MMI_SP.iFruit
{
    internal static class MechanicItemFactory
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void AddVehicleItem(UIMenu menu, string label, string description, bool enabled,
            Action onDeliver, int cost, Action onClose)
        {
            var item = new UIMenuItem(label, description);
            item.Enabled = enabled;
            menu.AddItem(item);

            if (!enabled) return;

            item.Activated += (sender, selectedItem) =>
            {
                if (!TryStartDelivery(cost)) return;
                onDeliver();
                Core.Instance.MechanicDeliveryRequested = true;
                onClose?.Invoke();
            };
        }

        internal static void AddDormantItem(UIMenu menu, DormantVehicle dormant, int cost, Action onClose)
        {
            string vehId = dormant.Data.Id;
            string vehName = VehicleIdentifier.GetLocalizedDisplayNameFromId(vehId);
            var pos = new GTA.Math.Vector3(dormant.Data.PosX, dormant.Data.PosY, dormant.Data.PosZ);
            float distance = Game.Player.Character.Position.DistanceTo(pos);

            // Variables comunes para todo el método
            string label;
            string description;
            bool enabled;

            // Si el durmiente está bloqueado, tratarlo como vehículo disponible (no mostrar "En depósito")
            if (dormant.Data.IsLocked)
            {
                label = $"{vehName} (Bloqueado)";
                description = $"Distancia: ~y~{distance:F0}m~w~ | Coste: ~g~{cost}$~w~";
                enabled = true;
            }
            else
            {
                // Detección de posible incautación policial
                bool isInPoliceImpound = VehiclesInGarage.IsPositionInPoliceImpound(Game.Player.Character.Position) && distance < 200f;

                if (isInPoliceImpound)
                {
                    label = $"{vehName} (En depósito)";
                    description = "Este vehículo está en el depósito policial.";
                    enabled = false;
                }
                else
                {
                    label = $"{vehName} (En depósito)";
                    description = $"Distancia: ~y~{distance:F0}m~w~ | Coste: ~g~{cost}$~w~";
                    enabled = true;
                }
            }

            AddVehicleItem(menu, label, description, enabled, () =>
            {
                var spawnResult = Dormancy.Core.Respawn(vehId);
                if (spawnResult.is_err())
                {
                    Notification.ShowMechanic("Mecánico", "No pude traer tu vehículo, algo salió mal.");
                    return;
                }

                Vehicle spawned = spawnResult.unwrap_or(null);
                BehindPlayer.Execute(spawned);
                Insurance.Delivery.Manager.RequestDelivery(spawned, cost, false, true,
                    Insurance.Observer.Manager.RecoveredVehList, Insurance.Observer.Manager.BlipsToRemove);
                Notification.ShowMechanic("Mecánico", "Marchando. Voy para allá, no te muevas.");
                MechanicSound.Play(MechanicSound.SoundFamily.Confirm);
            }, cost, onClose);
        }

        private static bool TryStartDelivery(int cost)
        {
            if (Insurance.Delivery.Manager.HasActiveDelivery())
            {
                Notification.ShowMechanic("Mecánico", "Aún estoy con otro encargo ¿Quién crees que soy? ¿Quicksilver?");
                MechanicSound.Play(MechanicSound.SoundFamily.Deny);
                return false;
            }

            if (Game.Player.Money < cost)
            {
                Notification.ShowMechanic("Mecánico", "Aquí no se fía. No trabajo gratis, llámame cuando tengas plata.");
                MechanicSound.Play(MechanicSound.SoundFamily.Deny);
                return false;
            }

            Game.Player.Money -= cost;
            return true;
        }
    }
}