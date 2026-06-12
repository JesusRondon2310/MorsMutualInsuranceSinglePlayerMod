using GTA;
using GTA.Native;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class DestroyedVehicle
    {
        internal static void Handle(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            string vehId = VehicleIdentifier.Get(currentVeh);
            string vehName = VehicleIdentifier.GetLocalizedDisplayName(currentVeh);

            Notification.ShowMMI("Información", $"Tu ~b~{vehName}~w~ ha sido destruido, llama a Mors Mutual o ve a nuestras oficinas para recuperarlo.");

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, Constants.SOUND_ARRIVE_TONE, Constants.SOUND_PHONE_SET, 1);

            Policies.Manager.MarkAsDestroyed(vehId).match<bool>(
                onOk: _ => true,
                onErr: error => {
                    Logger.Error($"Error al marcar vehículo como destruido: {error}");
                    return false;
                }
            );

            Policies.Manager.UpdateVehicleData(currentVeh).match<bool>(
                onOk: _ => true,
                onErr: error => {
                    Logger.Error($"Error al actualizar datos del vehículo destruido: {error}");
                    return false;
                }
            );

            currentVeh.IsPersistent = false;
            insuredVehList.RemoveAt(index);
            RecoveryBlipHandler.RemoveBlip(currentVeh, blipsToRemove);
        }
    }
}