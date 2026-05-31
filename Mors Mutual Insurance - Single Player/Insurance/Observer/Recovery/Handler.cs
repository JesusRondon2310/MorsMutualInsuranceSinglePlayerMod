using GTA;
using GTA.Native;
using MMI_SP.Config;
using MMI_SP.Helpers;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer.Recovery
{
    internal static class Handler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void OnVehicleRecovered(
            Vehicle veh,
            Blip blip,
            List<Vehicle> recoveredVehList,
            Dictionary<string, Blip> blipsToRemove)
        {
            if (!recoveredVehList.Contains(veh))
                recoveredVehList.Add(veh);

            string key = VehicleIdentifier.Get(veh);
            if (!blipsToRemove.ContainsValue(blip) && !blipsToRemove.ContainsKey(key))
                blipsToRemove.Add(key, blip);

            VehiclePersistence.SetPersistence(veh, true);
        }

        internal static void UpdateRecoveredVehicles(List<Vehicle> recoveredVehList)
        {
            for (int i = recoveredVehList.Count - 1; i >= 0; i--)
            {
                Vehicle recoveredVehicle = recoveredVehList[i];

                if (recoveredVehicle == null) continue;
                if (!recoveredVehicle.Exists()) continue;
                if (recoveredVehicle.IsDead) continue;
                if (Game.Player.LastVehicle == recoveredVehicle) continue;

                if (recoveredVehicle.IsAlive) Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet", 1);

                if (!ModSettings.PersistentVehicles) recoveredVehicle.IsPersistent = false;

                // La clave inmutable ya NO se quita aquí; se eliminará cuando el jugador suba al vehículo.
                recoveredVehList.RemoveAt(i);
            }
        }

        internal static void RemoveRecoveryKey(Vehicle playerVeh)
        {
            KeyManager.RemoveRecoveryKey(playerVeh);
        }

        internal static bool IsRecoveredAndNotDriven(Vehicle veh)
        {
            return KeyManager.IsRecoveredAndNotDriven(veh);
        }
    }
}