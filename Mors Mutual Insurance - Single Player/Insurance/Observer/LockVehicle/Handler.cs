using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class Handler
    {
        internal static bool SequenceRunning => Feedback.Running;
        internal static int SequenceStep => Feedback.Step;
        internal static int NextStepTime => Feedback.NextStepTime;
        internal static Vehicle SequenceVeh => Feedback.SequenceVeh;

        internal static void ShowHint(List<Vehicle> insuredVehList)
        {
            if (Feedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle nearest = StateChanger.FindNearestInsuredVehicle(player, insuredVehList);
            if (nearest == null) return;

            if (VehiclesInGarage.IsDefaultGarage(nearest)) return;

            string vehId = VehicleIdentifier.Get(nearest);
            bool isLocked = StateChanger.IsVehicleLocked(vehId);

            string action = isLocked ? "Desbloquear" : "Bloquear";
            string buttonPrompt = InputHandler.GetLockVehiclePrompt();
            GTA.UI.Screen.ShowHelpTextThisFrame($"Presiona {buttonPrompt} para {action} el vehículo");
        }

        internal static void Toggle(List<Vehicle> insuredVehList)
        {
            if (Feedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle target = StateChanger.FindNearestInsuredVehicle(player, insuredVehList);
            if (target == null) return;

            if (VehiclesInGarage.IsDefaultGarage(target))
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("No puedes bloquear un vehículo dentro de un garaje.");
                return;
            }

            string vehId = VehicleIdentifier.Get(target);
            StateChanger.ToggleLock(target, vehId);
            Feedback.Start(target);
        }

        internal static void Update(List<Vehicle> insuredVehList)
        {
            if (InputHandler.IsLockVehiclePressed()) Toggle(insuredVehList);
            Feedback.Update();
        }

        internal static void UpdateSequence() => Feedback.Update();
    }
}