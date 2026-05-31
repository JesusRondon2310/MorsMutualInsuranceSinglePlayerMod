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
        internal static void TryDespawn(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            if (HandleGarage(currentVeh, blipsToRemove)) return;
            HandleExterior(insurer, insuredVehList, blipsToRemove, index, currentVeh);
        }

        private static bool HandleGarage(Vehicle currentVeh, Dictionary<string, Blip> blipsToRemove)
        {
            if (!VehiclesInGarage.IsInAnyGarage(currentVeh)) return false;

            string vehId = VehicleIdentifier.Get(currentVeh);
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_some())
            {
                var data = ((Some<VehicleData>)dataOption).Value;
                var updatedData = new VehicleData(
                    data.Id, data.ModelName, data.Plate, data.PrimaryColor, data.SecondaryColor, data.IsDestroyed,
                    windowTint: data.WindowTint, wheelType: data.WheelType, wheelColor: data.WheelColor, tireSmokeColor: data.TireSmokeColor,
                    bulletproofTires: data.BulletproofTires, neonLeft: data.NeonLeft, neonRight: data.NeonRight, neonFront: data.NeonFront,
                    neonBack: data.NeonBack, neonColor: data.NeonColor, posX: data.PosX, posY: data.PosY, posZ: data.PosZ, heading: data.Heading,
                    mods: data.Mods, destroyedAt: data.DestroyedAt, isLocked: false, plateStyle: data.PlateStyle, customTires: data.CustomTires,
                    isDormant: false, isInGarage: true, vehicleType: data.VehicleType
                );
                DB.Core.Update(updatedData);
            }

            currentVeh.IsPersistent = false;
            BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
            return true;
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

            string recoveryKey = VehicleKey.From(data);
            bool isRecoveredVehicle = Manager.RecoveredVehicleKeys.Contains(recoveryKey);

            if (isRecoveredVehicle && !isPlayersVehicle)
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                AliveVehicle.Handle(insurer, currentVeh);
                return;
            }

            if (distance <= 600f)
            {
                currentVeh.IsPersistent = true;
                DB.Core.SetDormant(vehId, false);
                AliveVehicle.Handle(insurer, currentVeh);
                return;
            }

            if (!isPlayersVehicle)
            {
                insurer.UpdateVehicleData(currentVeh);
                var despawnResult = Dormancy.Core.Despawn(vehId, currentVeh);
                if (despawnResult.is_ok())
                {
                    BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
                    currentVeh.IsPersistent = false;
                    currentVeh.MarkAsNoLongerNeeded();
                    insuredVehList.RemoveAt(index);
                    currentVeh.Delete();
                    return;
                }
                Logger.Error($"[VehicleMonitor] Error al hacer despawn de {vehId}: {((Err<bool>)despawnResult).Message}");
            }

            AliveVehicle.Handle(insurer, currentVeh);
        }
    }
}