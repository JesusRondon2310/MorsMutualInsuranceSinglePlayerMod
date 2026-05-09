using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GTA;
using GTA.UI;

namespace MMI_SP.Common
{
    internal static class Utils
    {
        public static void ShowNotification(string icon, string title, string message, string description)
        {
            string fullMessage = $"{icon} {title}: {message}";
            Notification.PostTicker(fullMessage, isImportant: false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string GetCurrentMethod(int offset = 0)
        {
            var methodInfo = new StackTrace().GetFrame(1 + offset).GetMethod();
            var className = methodInfo.ReflectedType.Name;
            return $"{className}.{methodInfo.Name}";
        }

        /// <summary>
        /// Devuelve el identificador único del vehículo (hashModelo_matrícula).
        /// </summary>
        public static string GetVehicleIdentifier(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return string.Empty;
            return $"{veh.Model.Hash}_{veh.Mods.LicensePlate}";
        }
    }
}