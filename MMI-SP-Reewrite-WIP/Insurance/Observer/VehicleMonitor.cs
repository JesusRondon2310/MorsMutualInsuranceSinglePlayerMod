using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleMonitor
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void CheckForInsuredVehicles(Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            if (insurer == null) return;

            foreach (Vehicle veh in World.GetAllVehicles())
            {
                if (veh.IsDead) continue;
                if (insuredVehList.Contains(veh)) continue;

                bool isInsured = insurer.IsInsured(veh);

#pragma warning disable CS0618
                if (!isInsured && veh.Mods.LicensePlate == "46EEK572")
                    veh.Mods.LicensePlate = VehicleIdentifier.GetRandomNumberPlate();
#pragma warning restore CS0618

                if (!isInsured) continue;

                insuredVehList.Add(veh);
                string vehId = VehicleIdentifier.Get(veh);
                if (blipsToRemove.ContainsKey(vehId)) continue;

                var result = BlipManager.AddVehicleBlip(veh);
                if (result is Ok<Blip> ok) blipsToRemove[vehId] = ok.Value;
            }
        }

        internal static void UpdateInsurance(Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            for (int i = insuredVehList.Count - 1; i >= 0; i--)
            {
                Vehicle veh = insuredVehList[i];
                if (!veh.Exists()) { insuredVehList.RemoveAt(i); continue; }

                if (veh.IsDead)
                    HandleDestroyedVehicle(insurer, insuredVehList, blipsToRemove, i, veh);
                else
                    HandleAliveVehicle(insurer, veh);
            }
        }

        private static void HandleDestroyedVehicle(Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle veh)
        {
            string vehId = VehicleIdentifier.Get(veh);

            Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE",
                "Tu vehículo ha sido destruido. Llama a Mors Mutual para recuperarlo.", "");

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet", 1);

            insurer.MarkAsDestroyed(vehId);
            insurer.UpdateVehicleData(veh);

            veh.IsPersistent = false;
            insuredVehList.RemoveAt(index);
            BlipManager.RemoveRecoverBlip(veh, blipsToRemove);
        }

        private static void HandleAliveVehicle(Insurer insurer, Vehicle veh)
        {
            if (Game.Player.Character.CurrentVehicle == veh && GameplayCamera.IsRendering)
                insurer.UpdateVehicleData(veh);

            PersistenceManager.SetPersistence(veh, true);
        }
    }
}
