using System.Collections.Generic;
using System.Linq;
using GTA;
using MMI_SP.DB;
using MMI_SP.PatternMatching;

namespace MMI_SP.Dormancy
{
    public static class Core
    {
        internal static List<DormantVehicle> DormantVehicles = new List<DormantVehicle>();

        public static Result<bool> Despawn(string vehId, Vehicle veh)
        {
            if (string.IsNullOrEmpty(vehId)) return new Err<bool>("ID de vehículo no válido.");
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            return DespawnHandler.Execute(vehId, veh, DormantVehicles);
        }

        public static Result<bool> MarkAsDormant(string vehId)
        {
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return new Err<bool>("Vehículo no encontrado.");

            var data = ((Some<VehicleData>)dataOption).Value;

            // Anti‑duplicado: no añadir si ya existe un dormante con la misma placa
            if (DormantVehicles.Any(d => d.Data?.Plate == data.Plate && d.Data?.ModelName == data.ModelName))
                return new Err<bool>("El vehículo ya está en estado dormante.");

            var dormantData = new VehicleData(data.Id, data.ModelName, data.Plate, data.PrimaryColor, data.SecondaryColor, data.IsDestroyed,
                windowTint: data.WindowTint, wheelType: data.WheelType, wheelColor: data.WheelColor, tireSmokeColor: data.TireSmokeColor, 
                bulletproofTires: data.BulletproofTires, neonLeft: data.NeonLeft, neonRight: data.NeonRight, neonFront: data.NeonFront, 
                neonBack: data.NeonBack, neonColor: data.NeonColor, posX: data.PosX, posY: data.PosY, posZ: data.PosZ, heading: data.Heading,
                mods: data.Mods, destroyedAt: data.DestroyedAt, isLocked: data.IsLocked, plateStyle: data.PlateStyle, customTires: data.CustomTires,
                isDormant: true, isInGarage: false, vehicleType: data.VehicleType
            );

            DB.Core.Update(dormantData);

            var dormant = new DormantVehicle { Data = dormantData };
            DormantVehicles.Add(dormant);
            return new Ok<bool>(true);
        }

        internal static IReadOnlyList<DormantVehicle> GetDormantList() => DormantVehicles.AsReadOnly();

        public static Result<Vehicle> Respawn(string vehId)
        {
            if (string.IsNullOrEmpty(vehId)) return new Err<Vehicle>("ID de vehículo no válido.");
            return RespawnHandler.Execute(vehId, DormantVehicles);
        }
    }
}