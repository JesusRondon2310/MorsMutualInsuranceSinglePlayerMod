using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;
using MMI_SP.Insurance.Observer;   // Para Manager.InsuredVehList

namespace MMI_SP.Helpers
{
   public static class VehicleRecoveryHelper
   {
      public static void RecoverLostVehicles()
      {
         var lostVehicles = DB.Core.GetAll()
            .Where(v => !v.IsDestroyed && !v.IsDormant && !v.IsInGarage)
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

      public static void CleanDuplicatesInRadius(float radius = 600f)
      {
         var player = Game.Player.Character;
         var position = player.Position;
         int deleted = 0;

         var nearbyVehicles = World.GetNearbyVehicles(position, radius);
         foreach (var v in nearbyVehicles)
         {
            if (!v.Exists()) continue;
            if (v == player.CurrentVehicle) continue;
            if (Manager.InsuredVehList.Contains(v)) continue;

            // Construir la clave igual que ModelPlateKeyFrom (modelo_placa)
            string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash);
            string plate = v.Mods.LicensePlate;
            string vehicleKey = $"{modelName}_{plate}";

            var dataOpt = DB.Core.FindVehicle(vehicleKey);
            if (dataOpt.is_none()) continue;
            var data = dataOpt.unwrap_or(null);
            if (data == null || data.IsLocked == true) continue;

            // Limpieza y eliminación
            Function.Call(Hash.SET_VEHICLE_HAS_BEEN_OWNED_BY_PLAYER, v, false);
            Function.Call(Hash.SET_VEHICLE_IS_WANTED, v, false);
            Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, v, false);
            VehiclePersistence.SetPersistence(v, false);
            v.Delete();
            deleted++;
         }

         if (deleted > 0)
            Notification.ShowiFruit("Duplicados eliminados", $"Se eliminaron {deleted} vehículo(s) duplicado(s) en un radio de {radius}m.");
         else
            Notification.ShowiFruit("Sin duplicados", "No se encontraron vehículos duplicados en el área.");
      }
   }
}