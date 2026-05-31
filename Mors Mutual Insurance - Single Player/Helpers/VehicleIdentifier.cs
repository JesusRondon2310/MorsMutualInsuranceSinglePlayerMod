using GTA;
using GTA.Native;
using System;

namespace MMI_SP.Helpers
{
    internal static class VehicleIdentifier
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================

        /// Devuelve el identificador único del vehículo (hashModelo_matrícula).
        public static string Get(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return string.Empty;
            return $"{veh.Model.Hash}_{veh.Mods.LicensePlate}";
        }

        public static string GetRandomNumberPlate()
        {
            Random rnd = new Random();
            string plate = "";
            for (int i = 0; i < 8; i++)
                plate += rnd.Next(0, 10).ToString();
            return plate;
        }

        // Devuelve el nombre localizado del vehículo (ej. "Bravado Banshee 900R" en lugar de "BANSHEE2").
        public static string GetLocalizedDisplayName(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return "Desconocido";

            string name = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            if (string.IsNullOrEmpty(name)) return "Desconocido";

            string localized = Game.GetLocalizedString(name);
            return string.IsNullOrEmpty(localized) ? name : localized;
        }

        // Devuelve el nombre localizado del vehículo a partir de su ID (hashModelo_matrícula).
        public static string GetLocalizedDisplayNameFromId(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length < 1 || !int.TryParse(parts[0], out int hash)) return vehicleId;

            string name = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, hash);
            if (string.IsNullOrEmpty(name)) return vehicleId;

            string localized = Game.GetLocalizedString(name);
            return string.IsNullOrEmpty(localized) ? name : localized;
        }
    }
}