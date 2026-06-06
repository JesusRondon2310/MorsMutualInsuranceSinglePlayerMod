using GTA;
using GTA.Math;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Teleport;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
    internal static class VehicleTeleport
    {
        internal static void Execute(Vehicle veh, int cost, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove, 
        List<Data> incomingVehicles)
        {
            // 1. Teletransporte y frenado
            Teleport.BehindPlayer(veh);
            veh.Velocity = Vector3.Zero;
            veh.WorldRotationVelocity = Vector3.Zero;

            // 2. Congelar posición para evitar micro-rebotes
            veh.IsPositionFrozen = true;

            // 3. Timer asíncrono para descongelar y crear conductor
            string timerKey = $"TeleportFreeze_{veh.Handle}";
            Timers.StartCustomTimer(timerKey, Constants.TELEPORT_FREEZE_MS, () => {
                // Descongelar
                veh.IsPositionFrozen = false;

                // Crear conductor marcando la entrega como teletransportada
                var result = Incoming.BringVehicleTeleported(veh, cost, recoveredVehicle);
                result.match<bool>(
                    onOk: data => {
                        incomingVehicles.Add(data);
                        if (!recoveredVehicle) Bring.AddBlip(veh, blipsToRemove);
                        return true;
                    },
                    onErr: error => {
                        Logger.Error($"Error al crear conductor: {error}");
                        return false;
                    }
                );
            });
        }
    }
}