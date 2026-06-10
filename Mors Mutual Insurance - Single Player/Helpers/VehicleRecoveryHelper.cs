using GTA;
using GTA.Native;
using System.Linq;

namespace MMI_SP.Helpers
{
   public static class VehicleRecoveryHelper
   {
      // ==========================================
      // BLOQUE: Funciones
      // ==========================================
      public static void RecoverLostVehicles()
      {
         var lostVehicles = DB.Core.GetAll()
            .Where(v => !v.IsDestroyed && !v.IsDormant && !v.IsInNativeGarage && !v.IsInInteriorGarage)
            .ToList();

         if (lostVehicles.Count == Constants.NONE)
         {
            Notification.ShowiFruit("Sin novedades", "No se encontraron vehículos perdidos.");
            return;
         }

         int recoveredCount = Constants.NONE;
         int impoundedCount = Constants.NONE;

         foreach (var vd in lostVehicles)
         {
            var matches = World.GetAllVehicles()
               .Where(v => v.Mods.LicensePlate == vd.Plate && v.Model == new Model(vd.ModelName))
               .ToList();

            if (matches.Count == Constants.NONE)
            {
               Dormancy.Core.MarkAsDormant(vd.Id);
               recoveredCount++;
               continue;
            }

            int inImpound = matches.Count(v => VehiclesInGarage.IsPositionInPoliceImpound(v.Position));
            if (inImpound > Constants.NONE)
               impoundedCount++;
         }

         if (impoundedCount > Constants.NONE)
            Notification.ShowiFruit("Depósito policial", $"Tienes {impoundedCount} vehículo(s) en el depósito. Pagá la multa, moroso.");
         if (recoveredCount > Constants.NONE)
            Notification.ShowiFruit("Vehículos recuperados", $"Se marcaron {recoveredCount} vehículo(s) perdido(s) como dormantes. Llama al mecánico.");
         if (impoundedCount == Constants.NONE && recoveredCount == Constants.NONE)
            Notification.ShowiFruit("Todo en orden", "¿Tenías ganas de presionar el botón o qué?");
      }

      public static void CleanDuplicatesInRadius(float searchRadius = Constants.CHECK_VEHICLES_RADIUS, float deleteRadius = Constants.DELETE_VEHICLES_RADIUS)
      {
         var player = Game.Player.Character;
         var currentVeh = player.CurrentVehicle;
         if (currentVeh == null || !currentVeh.Exists())
         {
            Notification.ShowiFruit("Error", "Debes estar montado en el vehículo original.");
            return;
         }

         string targetModel = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, currentVeh.Model.Hash);
         string targetPlate = currentVeh.Mods.LicensePlate;
         var playerPos = player.Position;
         int deleted = Constants.NONE;

         var nearbyVehicles = World.GetNearbyVehicles(playerPos, searchRadius);

         foreach (var v in nearbyVehicles)
         {
            if (!v.Exists()) continue;
            if (v == currentVeh) continue;

            string candidateModel = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash);
            string candidatePlate = v.Mods.LicensePlate;

            if (candidateModel == targetModel && candidatePlate == targetPlate && v.Position.DistanceTo(playerPos) <= deleteRadius)
            {
               // Limpieza completa
               Function.Call(Hash.SET_VEHICLE_HAS_BEEN_OWNED_BY_PLAYER, v, false);
               Function.Call(Hash.SET_VEHICLE_IS_WANTED, v, false);
               Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, v, false);
               v.MarkAsNoLongerNeeded();
               v.Delete();
               deleted++;
            }
         }

         if (deleted > Constants.NONE)
            Notification.ShowiFruit("Información", $"Se eliminaron {deleted} vehículo(s) duplicado(s) a {deleteRadius} m.");
         else
            Notification.ShowiFruit("Sin duplicados", "Epaaa. Ten cuidadito, Wazowski; si se elimina tu vehículo original, no me hago responsable.");
      }
   }
}