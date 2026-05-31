using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Native;
using MMI_SP.PatternMatching;

namespace MMI_SP.DB
{
    internal static class VehicleDataBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<VehicleData> CreateFrom(Vehicle veh, string id)
        {
            if (veh == null || !veh.Exists()) return new Err<VehicleData>("El vehículo no existe.");

            if (string.IsNullOrEmpty(id)) return new Err<VehicleData>("ID de vehículo no válido.");

            string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            string plate = veh.Mods.LicensePlate;

            if (string.IsNullOrEmpty(plate)) return new Err<VehicleData>("El vehículo no tiene placa.");

            int plateStyle = Function.Call<int>(Hash.GET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, veh);

            int primaryColor = (int)veh.Mods.PrimaryColor;
            int secondaryColor = (int)veh.Mods.SecondaryColor;
            int windowTint = (int)veh.Mods.WindowTint;
            int wheelType = (int)veh.Mods.WheelType;
            int wheelColor = (int)veh.Mods.RimColor;
            int tireSmokeColor = GetTireSmokeColor(veh);

            veh.Mods.InstallModKit();

            float posX = veh.Position.X;
            float posY = veh.Position.Y;
            float posZ = veh.Position.Z;
            float heading = veh.Heading;

            var mods = new Dictionary<int, int>();
            for (int modType = 0; modType < 49; modType++)
            {
                int index = Function.Call<int>(Hash.GET_VEHICLE_MOD, veh, modType);
                if (index != -1) mods[modType] = index;
            }

            if (veh.Mods[VehicleToggleModType.Turbo].IsInstalled) mods[18] = 1;
            if (veh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled) mods[22] = 1;
            if (!mods.ContainsKey(10) && Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh, 10)) mods[10] = 1;

            bool canBurst = Function.Call<bool>(Hash.GET_VEHICLE_TYRES_CAN_BURST, veh);
            bool bulletproofTires = !canBurst;
            bool customTires = Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh, 23);

            bool neonLeft = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Left);
            bool neonRight = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Right);
            bool neonFront = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Front);
            bool neonBack = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Back);

            Color neonRaw = veh.Mods.NeonLightsColor;
            int neonColor = (neonLeft || neonRight || neonFront || neonBack)
                ? (neonRaw.R << 16) | (neonRaw.G << 8) | neonRaw.B
                : -1;

            string vehicleType = veh.Type.ToString();

            return new Ok<VehicleData>(new VehicleData(
                id, modelName, plate, primaryColor, secondaryColor, false,
                windowTint: windowTint,
                wheelType: wheelType,
                wheelColor: wheelColor,
                tireSmokeColor: tireSmokeColor,
                neonLeft: neonLeft,
                neonRight: neonRight,
                neonFront: neonFront,
                neonBack: neonBack,
                neonColor: neonColor,
                bulletproofTires: bulletproofTires,
                posX: posX, posY: posY, posZ: posZ, heading: heading,
                mods: mods,
                plateStyle: plateStyle,
                customTires: customTires,
                isLocked: false,
                isDormant: false,
                isInGarage: false,
                vehicleType: vehicleType));
        }

        private static int GetTireSmokeColor(Vehicle veh)
        {
            var r = new OutputArgument();
            var g = new OutputArgument();
            var b = new OutputArgument();
            Function.Call(Hash.GET_VEHICLE_TYRE_SMOKE_COLOR, veh, r, g, b);

            int rVal = r.GetResult<int>();
            int gVal = g.GetResult<int>();
            int bVal = b.GetResult<int>();

            if (rVal == 0 && gVal == 0 && bVal == 0) return -1;

            return (rVal << 16) | (gVal << 8) | bVal;
        }
    }
}