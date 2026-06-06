using GTA;
using GTA.Native;
using MMI_SP.DB;
using System;
using System.Linq;

namespace MMI_SP.Helpers.Spawn
{
    internal static class VehicleCustomizer
    {
        private static readonly VehicleToggleModType[] AllToggleModTypes = Enum.GetValues(typeof(VehicleToggleModType))
            .Cast<VehicleToggleModType>()
            .ToArray();

        // ==========================================
        // Aplica todas las personalizaciones al vehículo
        // ==========================================
        internal static void ApplyAll(Vehicle veh, VehicleData data)
        {
            if (veh == null || !veh.Exists() || data == null) return;

            // Identidad
            veh.Mods.LicensePlate = data.Plate;
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, veh, data.PlateStyle);

            // Colores
            veh.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            veh.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            veh.Mods.WindowTint = (VehicleWindowTint)data.WindowTint;

            veh.Mods.InstallModKit();

            // Modificaciones comunes (mods numéricos no toggle)
            foreach (var mod in data.Mods)
            {
                if (!Enum.IsDefined(typeof(VehicleToggleModType), mod.Key))
                    Function.Call(Hash.SET_VEHICLE_MOD, veh, mod.Key, mod.Value, false);
            }

            // Livery (mod 22)
            if (data.Mods.TryGetValue(22, out int liveryIndex))
                Function.Call(Hash.SET_VEHICLE_LIVERY, veh, liveryIndex);

            // Toggle mods (ej. neumáticos, turbo, etc.)
            foreach (VehicleToggleModType toggleType in AllToggleModTypes)
            {
                int modType = (int)toggleType;
                if (data.Mods.TryGetValue(modType, out int value) && value == 1)
                    veh.Mods[toggleType].IsInstalled = true;
            }

            // Ruedas
            veh.Mods.WheelType = (VehicleWheelType)data.WheelType;
            veh.Mods.RimColor = (VehicleColor)data.WheelColor;

            // Llantas antibalas
            if (data.BulletproofTires)
                Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, veh, false);

            // Neumáticos personalizados (mod 23)
            if (data.CustomTires)
            {
                int wheelMod = data.Mods.TryGetValue(23, out int modValue) ? modValue : 0;
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, wheelMod, true);
            }
            else if (data.Mods.ContainsKey(23))
            {
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, data.Mods[23], false);
            }

            // Humo de neumáticos
            if (data.TireSmokeColor >= 0)
            {
                veh.Mods[VehicleToggleModType.TireSmoke].IsInstalled = true;
                int r = (data.TireSmokeColor >> 16) & 0xFF;
                int g = (data.TireSmokeColor >> 8) & 0xFF;
                int b = data.TireSmokeColor & 0xFF;
                Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, veh, r, g, b);
            }

            // Neón
            if (data.NeonColor >= 0)
            {
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Left, data.NeonLeft);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Right, data.NeonRight);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Front, data.NeonFront);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Back, data.NeonBack);
                int r = (data.NeonColor >> 16) & 0xFF;
                int g = (data.NeonColor >> 8) & 0xFF;
                int b = data.NeonColor & 0xFF;
                veh.Mods.NeonLightsColor = System.Drawing.Color.FromArgb(r, g, b);
            }

            // Persistencia (evita que el juego borre el vehículo)
            VehiclePersistence.SetPersistence(veh, true);
        }
    }
}