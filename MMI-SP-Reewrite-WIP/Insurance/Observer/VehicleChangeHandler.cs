using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleChangeHandler
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        private static Vehicle _previousVehicle = null;

        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Handle(
            List<Vehicle> insuredVehList,
            List<Vehicle> recoveredVehList,
            Dictionary<string, Blip> blipsToRemove)
        {
            HandleVehicleChange(blipsToRemove);
        }

        private static void HandleVehicleChange(Dictionary<string, Blip> blipsToRemove)
        {
            if (_previousVehicle == Game.Player.Character.CurrentVehicle) return;

            Vehicle previous = _previousVehicle;
            _previousVehicle = Game.Player.Character.CurrentVehicle;

            if (_previousVehicle != null)
            {
                BlipManager.RemoveRecoverBlip(_previousVehicle, blipsToRemove);
            }
            else if (previous != null && previous.Exists())
            {
                ReponerBlip(previous, blipsToRemove);
            }
        }

        private static void ReponerBlip(Vehicle vehicle, Dictionary<string, Blip> blipsToRemove)
        {
            string vehId = VehicleIdentifier.Get(vehicle);
            if (!Insurer.Instance.IsInsured(vehicle)) return;
            if (blipsToRemove.ContainsKey(vehId)) return;

            var result = BlipManager.AddVehicleBlip(vehicle);
            if (result is Ok<Blip> ok) blipsToRemove[vehId] = ok.Value;
        }
    }
}
