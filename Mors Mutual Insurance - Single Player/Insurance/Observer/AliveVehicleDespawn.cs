using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class AliveVehicleDespawn
    {
        internal static void TryDespawn(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            if (Garage.Entry(currentVeh, blipsToRemove)) return;
            HandleExterior(insuredVehList, blipsToRemove, index, currentVeh);
        }

        private static bool IsPlayerInsideAnyInterior()
        {
            Ped playerPed = Game.Player.Character;
            int interiorId = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, playerPed);
            if (interiorId != 0 && interiorId != -1) return true;

            Vector3 pos = playerPed.Position;
            interiorId = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, pos.X, pos.Y, pos.Z);
            if (interiorId == 0 || interiorId == -1) return false;

            return Function.Call<bool>(Hash.IS_INTERIOR_READY, interiorId);
        }

        private static void HandleExterior(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            // 1. Si el jugador está en un interior, no hacer nada
            if (IsPlayerInsideAnyInterior()) return;

            // 2. Validar que la posición del vehículo sea válida
            if (currentVeh.Position == Vector3.Zero || currentVeh.Position.Length() < Constants.MIN_VALID_POSITION_LENGTH) return;

            // 3. Obtener distancia al jugador e identificar vehículo
            float distance = currentVeh.Position.DistanceTo(Game.Player.Character.Position);
            string vehId = VehicleIdentifier.Get(currentVeh);
            bool isPlayersVehicle = (Game.Player.Character.CurrentVehicle == currentVeh);

            // 4. Obtener datos del vehículo desde la BD
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return;
            VehicleData data = ((Some<VehicleData>)dataOption).Value;

            // 5. Manejo especial: vehículo en depósito policial
            if ((data.IsInNativeGarage || data.IsInInteriorGarage) && VehiclesInGarage.IsPositionInPoliceImpound(currentVeh.Position))
            {
                BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
                insuredVehList.RemoveAt(index);
                VehiclePersistence.SetPersistence(currentVeh, false);
                Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, currentVeh, false);
                currentVeh.Delete();
                return;
            }

            // 6. Proteger vehículo recién recuperado (si no lo conduce el jugador)
            string recoveryKey = VehicleKey.FullKeyFrom(data);
            if (Manager.RecoveredVehicleKeys.Contains(recoveryKey) && !isPlayersVehicle)
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                VehiclePersistence.SetPersistence(currentVeh, true);
                return;
            }

            // 7. Mantener despiertos los vehículos cercanos (< dormancy threshold)
            if (distance <= Constants.DORMANCY_THRESHOLD && !VehiclesInGarage.IsDefaultGarage(currentVeh))
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                VehiclePersistence.SetPersistence(currentVeh, true);
                Garage.Exit(currentVeh);
                return;
            }

            // 8. Si no es el vehículo del jugador, despawnearlo (dormancia)
            if (!isPlayersVehicle)
            {
                Policies.Manager.UpdateVehicleData(currentVeh);
                var despawnResult = Dormancy.Core.Despawn(vehId, currentVeh);
                if (despawnResult.is_ok())
                {
                    BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
                    insuredVehList.RemoveAt(index);
                    VehiclePersistence.SetPersistence(currentVeh, false);
                    Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, currentVeh, false);
                    currentVeh.Delete();
                }
                else {
                    Logger.Error($"[VehicleMonitor] Error al hacer despawn de {vehId}: {((Err<bool>)despawnResult).Message}");
                }
                return;
            }

            // 9. Por defecto, mantener persistencia
            VehiclePersistence.SetPersistence(currentVeh, true);
        }
    }
}