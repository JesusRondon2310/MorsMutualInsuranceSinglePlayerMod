using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class Handler
    {
        private const float MaxDistance = 2.5f;

        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static bool SequenceRunning => Feedback.Running;
        internal static int SequenceStep => Feedback.Step;
        internal static int NextStepTime => Feedback.NextStepTime;
        internal static Vehicle SequenceVeh => Feedback.SequenceVeh;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void ShowHint(List<Vehicle> insuredVehList)
        {
            if (Feedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle nearest = FindNearestInsuredVehicle(player, insuredVehList);
            if (nearest == null) return;

            if (VehiclesInGarage.IsDefaultGarage(nearest)) return;

            string vehId = VehicleIdentifier.Get(nearest);
            var dataOption = DB.Core.FindVehicle(vehId);
            bool isLocked = dataOption.match(
                onSome: data => data.IsLocked,
                onNone: () => false
            );

            string action = isLocked ? "Desbloquear" : "Bloquear";
            string buttonPrompt = InputHandler.GetLockVehiclePrompt();
            GTA.UI.Screen.ShowHelpTextThisFrame($"Presiona {buttonPrompt} para {action} el vehículo");
        }

        internal static void Toggle(List<Vehicle> insuredVehList, Insurer insurer)
        {
            if (Feedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle target = FindNearestInsuredVehicle(player, insuredVehList);
            if (target == null) return;

            if (VehiclesInGarage.IsDefaultGarage(target))
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("No puedes bloquear un vehículo dentro de un garaje.");
                return;
            }

            string vehId = VehicleIdentifier.Get(target);
            var dataOption = DB.Core.FindVehicle(vehId);

            bool currentlyLocked = dataOption.match(
                onSome: data => data.IsLocked,
                onNone: () => false
            );
            bool willBeLocked = !currentlyLocked;

            target.LockStatus = willBeLocked ? VehicleLockStatus.CannotEnter : VehicleLockStatus.Unlocked;
            if (willBeLocked) target.IsAlarmSet = true;

            DataPersistence.Persist(insurer, vehId, willBeLocked);
            Feedback.Start(target);
        }

        internal static void Update(List<Vehicle> insuredVehList, Insurer insurer)
        {
            if (InputHandler.IsLockVehiclePressed()) Toggle(insuredVehList, insurer);

            Feedback.Update();
        }

        internal static void UpdateSequence()
        {
            Feedback.Update();
        }

        private static Vehicle FindNearestInsuredVehicle(Ped player, List<Vehicle> insuredVehList)
        {
            Vehicle nearest = null;
            float minDist = MaxDistance;

            foreach (Vehicle veh in insuredVehList)
            {
                if (veh == null || !veh.Exists() || veh.IsDead) continue;
                float dist = player.Position.DistanceTo(veh.Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = veh;
                }
            }
            return nearest;
        }
    }
}