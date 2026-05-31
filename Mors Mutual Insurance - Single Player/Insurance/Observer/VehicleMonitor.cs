using GTA;
using GTA.Math;
using MMI_SP.DB;
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
        // BLOQUE: Funciones
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
                if (!currentVeh.Exists()) {
                    HandleMissingVehicle(insuredVehList, i, currentVeh);
                    continue;
                }

                if (currentVeh.IsDead) DestroyedVehicle.Handle(insurer, insuredVehList, blipsToRemove, i, currentVeh);
                
                else AliveVehicleDespawn.TryDespawn(insurer, insuredVehList, blipsToRemove, i, currentVeh);
            }
        }

        private static void HandleMissingVehicle(List<Vehicle> insuredVehList, int i, Vehicle currentVeh)
        {
            string vehId = VehicleIdentifier.Get(currentVeh);

            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) {
                insuredVehList.RemoveAt(i);
                return;
            }

            var data = dataOption.match<VehicleData>(
                onSome: vd => vd,
                onNone: () => null
            );

            if (data.IsDestroyed || data.IsDormant) {
                insuredVehList.RemoveAt(i);
                return;
            }

            if (data.IsLocked) {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            if (PoliceImpound.TryMarkAsImpounded(data, vehId, insuredVehList, i)) return;

            string recoveryKey = VehicleKey.From(data);
            bool isRecoveredVehicle = Manager.RecoveredVehicleKeys.Contains(recoveryKey);

            Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            float distanceToPlayer = Game.Player.Character.Position.DistanceTo(lastPos);

            if (distanceToPlayer < 600f || (isRecoveredVehicle && !Game.Player.Character.IsInVehicle(currentVeh))) {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            Dormancy.Core.MarkAsDormant(vehId);
            insuredVehList.RemoveAt(i);
            Logger.Warning($"[VehicleMonitor] Despawn involuntario del engine: {vehId}");
        }
    }
}