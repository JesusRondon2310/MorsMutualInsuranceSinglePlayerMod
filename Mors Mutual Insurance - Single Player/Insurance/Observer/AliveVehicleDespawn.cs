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
        internal static void TryDespawn(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, 
            Vehicle currentVeh)
        {
            if (Garage.Entry(currentVeh, blipsToRemove)) return;
            HandleExterior(insurer, insuredVehList, blipsToRemove, index, currentVeh);
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

        private static void HandleExterior(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            if (IsPlayerInsideAnyInterior()) return;

            if (currentVeh.Position == Vector3.Zero || currentVeh.Position.Length() < 1f) return;

            float distance = currentVeh.Position.DistanceTo(Game.Player.Character.Position);
            string vehId = VehicleIdentifier.Get(currentVeh);
            bool isPlayersVehicle = Game.Player.Character.CurrentVehicle == currentVeh;

            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return;
            VehicleData data = dataOption.match<VehicleData>(
                onSome: vd => vd,
                onNone: () => null
            );

            if (data.IsInGarage && VehiclesInGarage.IsPositionInPoliceImpound(currentVeh.Position))
            {
                BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
                insuredVehList.RemoveAt(index);
                VehiclePersistence.SetPersistence(currentVeh, false);
                Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, currentVeh, false);
                currentVeh.Delete();
                return;
            }

            string recoveryKey = VehicleKey.FullKeyFrom(data);
            bool isRecoveredVehicle = Manager.RecoveredVehicleKeys.Contains(recoveryKey);

            if (isRecoveredVehicle && !isPlayersVehicle)
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                AliveVehicle.Handle(insurer, currentVeh);
                return;
            }

            if (distance <= 600f && !VehiclesInGarage.IsDefaultGarage(currentVeh))
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                AliveVehicle.Handle(insurer, currentVeh);
                Garage.Exit(currentVeh);
                return;
            }

            if (!isPlayersVehicle)
            {
                insurer.UpdateVehicleData(currentVeh);
                var despawnResult = Dormancy.Core.Despawn(vehId, currentVeh);
                if (despawnResult.is_ok())
                {
                    BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
                    insuredVehList.RemoveAt(index);
                    VehiclePersistence.SetPersistence(currentVeh, false);
                    Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, currentVeh, false);
                    currentVeh.Delete();
                    return;
                }
                Logger.Error($"[VehicleMonitor] Error al hacer despawn de {vehId}: {((Err<bool>)despawnResult).Message}");
            }

            AliveVehicle.Handle(insurer, currentVeh);
        }
    }
}