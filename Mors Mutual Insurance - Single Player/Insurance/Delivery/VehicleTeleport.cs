using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Debug;
using MMI_SP.Helpers.Teleport;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
   internal static class VehicleTeleport
   {
      // ==========================================
      // BLOQUE: Funciones
      // ==========================================
      internal static void Execute(Vehicle veh, int cost, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove,
      List<Data> incomingVehicles)
      {
         // Teletransporte y frenado absoluto (parte del 20% mutable permitido)
         Teleport.BehindPlayer(veh);
         veh.Velocity = Vector3.Zero;
         veh.WorldRotationVelocity = Vector3.Zero;

         Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, veh);
         CreateDriverAndFinalize(veh, cost, recoveredVehicle, blipsToRemove, incomingVehicles);
      }

      private static void CreateDriverAndFinalize(Vehicle veh, int cost, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove,
       List<Data> incomingVehicles)
      {
         // ✅ APLICADA REGLA 4.2: Pattern matching con Result<T>
         var result = Incoming.BringVehicleTeleported(veh, cost, recoveredVehicle);

         // ✅ APLICADA REGLA 7.1: match<TResult> con tipo explícito (bool)
         // ✅ APLICADA REGLA 7.6: No ignoramos el Result, lo procesamos exhaustivamente
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
      }
   }
}