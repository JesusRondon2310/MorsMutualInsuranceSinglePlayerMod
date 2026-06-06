using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;

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

      public static void CleanDuplicatesInImpound()
      {
         Vector3 impoundCenter = VehiclesInGarage.PoliceImpoundPositions[Constants.FIRST_INDEX];
         float radius = Constants.LS_POLICE_IMPOUND_RADIUS;

         int deleted = Constants.NONE;
         var vehiclesInArea = World.GetAllVehicles()
            .Where(v => v.Position.DistanceTo(impoundCenter) <= radius)
            .ToList();

         foreach (var v in vehiclesInArea)
         {
            string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash);
            string plate = v.Mods.LicensePlate;

            var dataOption = DB.Core.GetAll().FirstOrDefault(d => d.ModelName == modelName && d.Plate == plate);

            if (dataOption != null)
            {
            Function.Call(Hash.SET_VEHICLE_HAS_BEEN_OWNED_BY_PLAYER, v, false);
            Function.Call(Hash.SET_VEHICLE_IS_WANTED, v, false);
            Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, v, false);
            VehiclePersistence.SetPersistence(v, false);
               v.Delete();
               deleted++;
            }
         }

         if (deleted > Constants.NONE) Notification.ShowiFruit("Depósito limpiado", $"Se eliminaron {deleted} vehículo(s) duplicado(s).");

         else Notification.ShowiFruit("Sin duplicados", "No se encontraron vehículos duplicados en el depósito.");
      }
   }
}