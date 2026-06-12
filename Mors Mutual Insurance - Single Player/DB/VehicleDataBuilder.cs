using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.DB
{
    internal static class VehicleDataBuilder
    {
        // ==========================================
        // BLOQUE: Métodos auxiliares privados
        // ==========================================
        private static Dictionary<int, int> GetModsDictionary(Vehicle veh)
        {
            var mods = new Dictionary<int, int>();
            for (int modType = Constants.FIRST_INDEX; modType < Constants.MAX_VEHICLE_MOD_TYPE; modType++) {
                int index = Function.Call<int>(Hash.GET_VEHICLE_MOD, veh, modType);
                if (index != Constants.NO_MOD) mods[modType] = index;
            }

            if (veh.Mods[VehicleToggleModType.Turbo].IsInstalled) mods[Constants.TURBO_MOD_INDEX] = Constants.MOD_INSTALLED;
            if (veh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled) mods[Constants.XENON_MOD_INDEX] = Constants.MOD_INSTALLED;
            if (!mods.ContainsKey(Constants.VARIATION_MOD_10) && Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh, Constants.VARIATION_MOD_10))
                mods[Constants.VARIATION_MOD_10] = Constants.MOD_INSTALLED;

            return mods;
        }

        private static (bool left, bool right, bool front, bool back, int color) GetNeonData(Vehicle veh)
        {
            bool left = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Left);
            bool right = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Right);
            bool front = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Front);
            bool back = veh.Mods.IsNeonLightsOn(VehicleNeonLight.Back);

            Color neonRaw = veh.Mods.NeonLightsColor;
            int color = (left || right || front || back)
                ? (neonRaw.R << 16) | (neonRaw.G << 8) | neonRaw.B
                : Constants.NO_COLOR;

            return (left, right, front, back, color);
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

            if (rVal == Constants.ZERO && gVal == Constants.ZERO && bVal == Constants.ZERO) return Constants.NO_COLOR;

            return (rVal << 16) | (gVal << 8) | bVal;
        }

        // ==========================================
        // BLOQUE: Función principal
        // ==========================================
        internal static Result<VehicleData> CreateFrom(Vehicle veh, string id)
        {
            if (veh == null || !veh.Exists()) return new Err<VehicleData>("El vehículo no existe.");
            if (string.IsNullOrEmpty(id)) return new Err<VehicleData>("ID de vehículo no válido.");

            // Instalar kit de modificaciones ANTES de leer cualquier mod
            veh.Mods.InstallModKit();

            // Datos básicos inmutables
            string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            string plate = veh.Mods.LicensePlate;
            if (string.IsNullOrEmpty(plate)) return new Err<VehicleData>("El vehículo no tiene placa.");

            int plateStyle = Function.Call<int>(Hash.GET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, veh);

            // Colores y modificaciones
            int primaryColor = (int)veh.Mods.PrimaryColor;
            int secondaryColor = (int)veh.Mods.SecondaryColor;
            int windowTint = (int)veh.Mods.WindowTint;
            int wheelType = (int)veh.Mods.WheelType;
            int wheelColor = (int)veh.Mods.RimColor;
            int tireSmokeColor = GetTireSmokeColor(veh);
            bool bulletproofTires = !Function.Call<bool>(Hash.GET_VEHICLE_TYRES_CAN_BURST, veh);
            bool customTires = Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh, Constants.CUSTOM_TIRES_MOD_INDEX);

            // Modificaciones (mods)
            var mods = GetModsDictionary(veh);

            // Neón
            var (neonLeft, neonRight, neonFront, neonBack, neonColor) = GetNeonData(veh);

            // Posición y heading
            float posX = veh.Position.X;
            float posY = veh.Position.Y;
            float posZ = veh.Position.Z;
            float heading = veh.Heading;

            // Tipo de vehículo
            string vehicleType = veh.Type.ToString();

            // Construir el DTO
            return new Ok<VehicleData>(new VehicleData(id, modelName, plate, primaryColor, secondaryColor, isDestroyed: false, windowTint: windowTint,
                wheelType: wheelType, wheelColor: wheelColor, tireSmokeColor: tireSmokeColor, neonLeft: neonLeft, neonRight: neonRight,
                neonFront: neonFront, neonBack: neonBack, neonColor: neonColor, bulletproofTires: bulletproofTires, posX: posX, posY: posY,
                posZ: posZ, heading: heading, mods: mods, plateStyle: plateStyle, customTires: customTires, isLocked: false,
                isDormant: false, isInNativeGarage: false, isInInteriorGarage: false, vehicleType: vehicleType));
        }
    }
}