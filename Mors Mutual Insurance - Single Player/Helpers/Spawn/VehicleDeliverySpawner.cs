using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;
using System;
using System.Linq;

namespace MMI_SP.Helpers.Spawn
{
    public static class VehicleDeliverySpawner
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly VehicleToggleModType[] AllToggleModTypes = Enum.GetValues(typeof(VehicleToggleModType))
            .Cast<VehicleToggleModType>()
            .ToArray();

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<Vehicle> SpawnVehicleForDelivery(VehicleData data, bool validatePosition = true)
        {
            if (data == null) return new Err<Vehicle>("Datos del vehículo no válidos.");

            Model model = new Model(data.ModelName);
            if (!model.Request(3000) && !model.Request(3000)) {
                model.MarkAsNoLongerNeeded();
                return new Err<Vehicle>($"No se pudo cargar el modelo {data.ModelName}. Dale las gracias a la kk de C#.");
            }

            Vector3 spawnPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            float heading = data.Heading;

            if (validatePosition) {
                var nodeResult = RoadSpawnHandler.FindNode(spawnPos.X, spawnPos.Y, spawnPos.Z);
                if (nodeResult.is_ok()) spawnPos = ((Ok<Vector3>)nodeResult).Value;

                heading = GetValidOrientation.FromNode(spawnPos, data.Heading);
            }

            Vehicle veh = World.CreateVehicle(model, spawnPos, heading);
            if (veh == null || !veh.Exists()) return new Err<Vehicle>("No se pudo crear el vehículo.");

            // Identidad
            veh.Mods.LicensePlate = data.Plate;
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, veh, data.PlateStyle);

            // Colores
            veh.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            veh.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            veh.Mods.WindowTint = (VehicleWindowTint)data.WindowTint;

            veh.Mods.InstallModKit();

            // Modificaciones
            foreach (var mod in data.Mods)
            {
                if (!Enum.IsDefined(typeof(VehicleToggleModType), mod.Key)) Function.Call(Hash.SET_VEHICLE_MOD, veh, mod.Key, mod.Value, false);
            }

            if (data.Mods.TryGetValue(22, out int liveryIndex)) Function.Call(Hash.SET_VEHICLE_LIVERY, veh, liveryIndex);

            foreach (VehicleToggleModType toggleType in AllToggleModTypes)
            {
                int modType = (int)toggleType;
                if (data.Mods.TryGetValue(modType, out int value) && value == 1)
                    veh.Mods[toggleType].IsInstalled = true;
            }

            // Ruedas
            veh.Mods.WheelType = (VehicleWheelType)data.WheelType;
            veh.Mods.RimColor = (VehicleColor)data.WheelColor;
            VehiclePersistence.SetPersistence(veh, true);

            if (data.BulletproofTires) Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, veh, false);

            // Neumáticos personalizados
            if (data.CustomTires)
            {
                int wheelMod = data.Mods.TryGetValue(23, out int modValue) ? modValue : 0;
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, wheelMod, true);
            }
            else if (data.Mods.ContainsKey(23)) {
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, data.Mods[23], false);
            }

            // Humo de neumáticos
            if (data.TireSmokeColor >= 0) {
                veh.Mods[VehicleToggleModType.TireSmoke].IsInstalled = true;
                int r = (data.TireSmokeColor >> 16) & 0xFF;
                int g = (data.TireSmokeColor >> 8) & 0xFF;
                int b = data.TireSmokeColor & 0xFF;
                Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, veh, r, g, b);
            }

            // Neón
            if (data.NeonColor >= 0) {
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Left, data.NeonLeft);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Right, data.NeonRight);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Front, data.NeonFront);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Back, data.NeonBack);
                int r = (data.NeonColor >> 16) & 0xFF;
                int g = (data.NeonColor >> 8) & 0xFF;
                int b = data.NeonColor & 0xFF;
                veh.Mods.NeonLightsColor = System.Drawing.Color.FromArgb(r, g, b);
            }

            if (data.IsLocked) veh.LockStatus = VehicleLockStatus.CannotEnter;

            return new Ok<Vehicle>(veh);
        }
    }
}