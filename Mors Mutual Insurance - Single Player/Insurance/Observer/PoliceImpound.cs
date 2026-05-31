using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class PoliceImpound
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static bool TryMarkAsImpounded(VehicleData data, string vehId, List<Vehicle> insuredVehList, int index)
        {
            Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            if (!VehiclesInGarage.IsPositionInPoliceImpound(lastPos))
                return false;

            var updatedData = new VehicleData(
                data.Id, data.ModelName, data.Plate,
                data.PrimaryColor, data.SecondaryColor, data.IsDestroyed,
                windowTint: data.WindowTint, wheelType: data.WheelType, wheelColor: data.WheelColor,
                tireSmokeColor: data.TireSmokeColor, bulletproofTires: data.BulletproofTires,
                neonLeft: data.NeonLeft, neonRight: data.NeonRight, neonFront: data.NeonFront, neonBack: data.NeonBack,
                neonColor: data.NeonColor,
                posX: data.PosX, posY: data.PosY, posZ: data.PosZ, heading: data.Heading,
                mods: data.Mods,
                destroyedAt: data.DestroyedAt, isLocked: data.IsLocked,
                plateStyle: data.PlateStyle, customTires: data.CustomTires,
                isDormant: false,
                isInGarage: true,
                vehicleType: data.VehicleType);

            DB.Core.Update(updatedData);
            insuredVehList.RemoveAt(index);
            return true;
        }
    }
}