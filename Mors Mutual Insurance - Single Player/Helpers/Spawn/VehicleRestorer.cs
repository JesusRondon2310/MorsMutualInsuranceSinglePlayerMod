using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Helpers.Spawn
{
   public static class VehicleRestorer
   {
      // ==========================================
      // BLOQUE: Restauración al cargar partida
      // ==========================================
      public static void ExecuteRestorationFrom(VehicleData data, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
      {
         // 1. Validación inicial
         if (data.IsDestroyed) return;

         // 2. Intentar cargar el modelo
         Model model = new Model(data.ModelName);
         bool modelLoaded = model.Request(Constants.MEDIUM_TIMEOUT_MS);

         // 3. CASO A: Modelo cargado correctamente
         if (modelLoaded)
         {
            model.MarkAsNoLongerNeeded();

            // 3.1 Limpiar el garaje nativo (si existe un vehículo base)
            Vector3 garagePos = new Vector3(data.PosX, data.PosY, data.PosZ);
            Vehicle garageVehicle = World.GetClosestVehicle(garagePos, Constants.GARAGE_SEARCH_RADIUS);
            if (garageVehicle != null && garageVehicle.Exists() && VehiclesInGarage.IsDefaultGarage(garageVehicle))
            {
               Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, garageVehicle, false);
               garageVehicle.Delete();
            }

            // 3.2 Buscar si ya existe un vehículo duplicado (mismo modelo+placa) a menos de 600m
            string searchKey = VehicleKey.ModelPlateKeyFrom(data);
            Vector3 playerPos = Game.Player.Character.Position;
            Vehicle duplicateVehicle = World.GetAllVehicles()
               .FirstOrDefault(v => v.Exists()
                  && v != garageVehicle
                  && $"{v.Model}_{v.Mods.LicensePlate}" == searchKey
                  && v.Position.DistanceTo(playerPos) < Constants.DORMANCY_THRESHOLD);

            // 3.3 Si hay duplicado, lo adoptamos y actualizamos la BD
            if (duplicateVehicle != null && duplicateVehicle.Exists())
            {
               Logger.Warning($"Vehículo duplicado detectado (KeyPair: {searchKey}) a " +
                  $"<{Constants.DORMANCY_THRESHOLD}m. Se usará ese en lugar de spawnear.");
               insuredVehList.Add(duplicateVehicle);

               var blipResult = VehicleBlipHandler.Create(duplicateVehicle);
               if (blipResult is Ok<Blip> okBlip) blipsToRemove[data.Id] = okBlip.Value;

               var updatedData = data.With(d =>
               {
                  d.PosX = duplicateVehicle.Position.X;
                  d.PosY = duplicateVehicle.Position.Y;
                  d.PosZ = duplicateVehicle.Position.Z;
                  d.Heading = duplicateVehicle.Heading;
                  d.IsInGarage = false;
               });
               DB.Core.Update(updatedData);
               return; // Todo listo, no spawnear nuevo
            }

            // 3.4 No hay duplicado → spawnear el vehículo desde la BD
            var spawnResult = VehicleSpawnManager.SpawnVehicle(data);
            spawnResult.match<bool>(
               onOk: spawned =>
               {
                  insuredVehList.Add(spawned);
                  var blipResult = VehicleBlipHandler.Create(spawned);
                  if (blipResult is Ok<Blip> okBlip)
                     blipsToRemove[data.Id] = okBlip.Value;
                  return true;
               },
               onErr: error =>
               {
                  Logger.Error($"Error al restaurar vehículo {data.ModelName}: {error}");
                  return false;
               }
            );
            return;
         }

         // 4. CASO B: Modelo NO cargado (fallback)
         model.MarkAsNoLongerNeeded();
         Logger.Warning($"No se pudo cargar el modelo {data.ModelName} - {data.Plate}. Se mantiene el vehículo original.");

         Vector3 fallbackPos = new Vector3(data.PosX, data.PosY, data.PosZ);
         Vehicle existingVehicle = World.GetClosestVehicle(fallbackPos, Constants.GARAGE_SEARCH_RADIUS);
         if (existingVehicle != null && existingVehicle.Exists())
         {
            insuredVehList.Add(existingVehicle);
            var blipResult = VehicleBlipHandler.Create(existingVehicle);
            if (blipResult is Ok<Blip> okBlip) blipsToRemove[data.Id] = okBlip.Value;
         }
      }
   }
}