using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class RecoveryManager
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

            PersistenceManager.SetPersistence(veh, true);
        }

        internal static void UpdateRecoveredVehicles(List<Vehicle> recoveredVehList)
        {
            for (int i = recoveredVehList.Count - 1; i >= 0; i--)
            {
                Vehicle veh = recoveredVehList[i];

                if (veh == null) continue;
                if (!veh.Exists()) continue;
                if (veh.IsDead) continue;
                if (Game.Player.LastVehicle == veh) continue;

                if (veh.IsAlive)
                {
                    Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE",
                        "VEHÍCULO RECUPERADO", "Tu vehículo ha sido recuperado. Pásate por el depósito.");

                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet", 1);
                }

                if (!Config.PersistentVehicles)
                    veh.IsPersistent = false;

                recoveredVehList.RemoveAt(i);
            }
        }
    }
}
