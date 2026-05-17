using GTA;
using System;

namespace MMI_SP.Helpers
{
    internal static class VehicleIdentifier
    {
        private static readonly Random _rng = new Random();
        private static readonly string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Get(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return string.Empty;
#pragma warning disable CS0618
            return $"{veh.Model.Hash}_{veh.Mods.LicensePlate}";
#pragma warning restore CS0618
        }

        public static string GetRandomNumberPlate()
        {
            char[] plate = new char[8];
            for (int i = 0; i < plate.Length; i++)
                plate[i] = _chars[_rng.Next(_chars.Length)];
            return new string(plate);
        }
    }
}