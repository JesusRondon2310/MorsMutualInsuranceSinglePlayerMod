using GTA;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleMonitor
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static bool _wasInsideGarage = false;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void CheckForInsuredVehicles(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            if (insurer == null) return;

            HashSet<string> trackedIds = new HashSet<string>(
                insuredVehList.Where(v => v.Exists()).Select(v => VehicleIdentifier.Get(v)).Where(id => !string.IsNullOrEmpty(id)));

            Vehicle[] allVehicles = World.GetAllVehicles();

            foreach (Vehicle veh in allVehicles)
            {
                if (veh.IsDead) continue;
                if (trackedIds.Contains(VehicleIdentifier.Get(veh))) continue;
                if (veh == Game.Player.Character.CurrentVehicle && veh.Mods.LicensePlate == "46EEK572")
                    veh.Mods.LicensePlate = VehicleIdentifier.GetRandomNumberPlate();

                bool isInsured = insurer.IsInsured(veh);

                if (isInsured)
                {
                    string vehId = VehicleIdentifier.Get(veh);
                    if (trackedIds.Contains(vehId)) continue;

                    insuredVehList.Add(veh);
                    trackedIds.Add(vehId);

                    if (!blipsToRemove.ContainsKey(vehId))
                        VehicleBlipHandler.Create(veh).match<bool>(
                            onOk: blip => {
                                blipsToRemove[vehId] = blip;
                                return true;
                            },
                            onErr: error => {
                                Logger.Error($"Error al crear blip del vehículo: {error}");
                                return false;
                            }
                        );
                }
            }
        }

        internal static void UpdateInsurance(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            for (int i = insuredVehList.Count - 1; i >= 0; i--)
            {
                Vehicle currentVeh = insuredVehList[i];
                if (!currentVeh.Exists())
                {
                    MissingVehicles.Handle(insuredVehList, i, currentVeh);
                    continue;
                }

                if (currentVeh.IsDead) DestroyedVehicle.Handle(insurer, insuredVehList, blipsToRemove, i, currentVeh);

                else AliveVehicleDespawn.TryDespawn(insurer, insuredVehList, blipsToRemove, i, currentVeh);
            }

            // Detectar salida del garaje interior a pie (usando la posición del jugador)
            CheckGarageExitOnFoot(insuredVehList, blipsToRemove);
        }

        private static void CheckGarageExitOnFoot(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            // CORREGIDO: usar la posición del jugador, no su vehículo
            bool isInside = VehiclesInGarage.IsPositionInInteriorGarage(Game.Player.Character.Position);
            if (_wasInsideGarage && !isInside)
            {
                Helpers.Spawn.InteriorVehicleRestorer.OnExitFoot(insuredVehList, blipsToRemove);
            }
            _wasInsideGarage = isInside;
        }
    }
}