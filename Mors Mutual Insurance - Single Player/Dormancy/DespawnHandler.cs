using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Observer;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Dormancy
{
    internal static class DespawnHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<bool> Execute(string vehId, Vehicle veh, List<DormantVehicle> dormantList)
        {
            if (string.IsNullOrEmpty(vehId)) return new Err<bool>("ID de vehículo no válido.");
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            if (dormantList == null) return new Err<bool>("Lista de dormantes nula.");

            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return new Err<bool>("Vehículo no encontrado en la base de datos.");
            VehicleData data = dataOption.match<VehicleData>(
                onSome: vd => vd,
                onNone: () => null
            );

            if (data.IsLocked) return new Err<bool>("Vehículo bloqueado, no se puede dormir.");

            if (data.IsInGarage) return new Err<bool>("Vehículo en garaje, no se puede dormir.");

            Vector3 dbPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            float dbDistance = dbPos.DistanceTo(Game.Player.Character.Position);
            if (dbDistance < 600f) return new Err<bool>("La posición guardada del vehículo está cerca del jugador, no se duerme.");

            string recoveryKey = VehicleKey.From(data);
            if (Manager.RecoveredVehicleKeys.Contains(recoveryKey) && !Game.Player.Character.IsInVehicle(veh))
                return new Err<bool>("Vehículo recién reclamado, no se puede dormir hasta que el jugador lo recoja.");

            if (veh.Position == Vector3.Zero || veh.Position.Length() < 1f) return new Err<bool>("Posición del vehículo inválida.");

            float distanceToPlayer = veh.Position.DistanceTo(Game.Player.Character.Position);
            if (distanceToPlayer < 600f) return new Err<bool>("Vehículo demasiado cerca del jugador para dormir.");

            if (dormantList.Any(d => d.Data?.Plate == data.Plate && d.Data?.ModelName == data.ModelName))
                return new Err<bool>("El vehículo ya está en estado dormante.");

            Vector3 lastPosition = veh.Position;

            var dormantData = new VehicleData(
                data.Id, data.ModelName, data.Plate, data.PrimaryColor, data.SecondaryColor, data.IsDestroyed, windowTint: data.WindowTint, 
                wheelType: data.WheelType, wheelColor: data.WheelColor, tireSmokeColor: data.TireSmokeColor, bulletproofTires: data.BulletproofTires,
                neonLeft: data.NeonLeft, neonRight: data.NeonRight, neonFront: data.NeonFront, neonBack: data.NeonBack, neonColor: data.NeonColor,
                posX: lastPosition.X, posY: lastPosition.Y, posZ: lastPosition.Z, heading: data.Heading, mods: data.Mods, destroyedAt: data.DestroyedAt,
                isLocked: data.IsLocked, plateStyle: data.PlateStyle, customTires: data.CustomTires, isDormant: true, isInGarage: false,
                vehicleType: data.VehicleType
            );

            var updateResult = DB.Core.Update(dormantData);
            if (updateResult.is_err()) return new Err<bool>(((Err<bool>)updateResult).Message);

            var dormant = new DormantVehicle { Data = dormantData };
            dormantList.Add(dormant);
            return new Ok<bool>(true);
        }
    }
}