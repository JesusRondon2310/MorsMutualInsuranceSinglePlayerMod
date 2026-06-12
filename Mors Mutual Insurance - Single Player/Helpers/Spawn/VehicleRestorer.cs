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

         // 3A. CASO A: Modelo cargado correctamente
         if (modelLoaded)
         {
            // 3A.1 Buscamos si ya existe un vehículo duplicado (mismo modelo+placa) a menos de 600m
            string searchKey = VehicleKey.ModelPlateKeyFrom(data);
            Vector3 garagePos = new Vector3(data.PosX, data.PosY, data.PosZ);
            Vehicle garageVehicle = World.GetClosestVehicle(garagePos, Constants.GARAGE_SEARCH_RADIUS);
            Vector3 playerPos = Game.Player.Character.Position;
            Vehicle duplicateVehicle = World.GetAllVehicles()
               .FirstOrDefault(v => v.Exists()
                  && v != garageVehicle
                  && $"{Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash)}_{v.Mods.LicensePlate}" == searchKey
                  && v.Position.DistanceTo(playerPos) < Constants.DORMANCY_THRESHOLD);

            // 3A.2 Si hay duplicado, NO lo adoptamos, solo eliminamos el garageVehicle (si es nativo) y salimos.
            if (duplicateVehicle != null && duplicateVehicle.Exists())
            {
               Logger.Warning($"Vehículo duplicado detectado (KeyPair: {searchKey}) a " +
                   $"<{Constants.DORMANCY_THRESHOLD}m. Se eliminará el coche base del garaje nativo.");

               // Eliminar garageVehicle si existe, es válido y es un garaje nativo
               if (garageVehicle != null && garageVehicle.Exists() && VehiclesInGarage.IsDefaultGarage(garageVehicle)) {
                  Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, garageVehicle, false);
                  garageVehicle.MarkAsNoLongerNeeded();
                  garageVehicle.Delete();
               }

               // No se añade el duplicado a insuredVehList, no se crea blip, no se actualiza BD.
               // VehicleMonitor se encargará de detectarlo automáticamente.
               model.MarkAsNoLongerNeeded();
               return;
            }

            // 3A.3 No hay duplicado → verificar coincidencia de garageVehicle
            if (garageVehicle != null && garageVehicle.Exists() && VehiclesInGarage.IsDefaultGarage(garageVehicle))
            {
               string garageModel = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, garageVehicle.Model.Hash);
               string garagePlate = garageVehicle.Mods.LicensePlate;
               if (garageModel == data.ModelName && garagePlate == data.Plate)
               {
                  // Coincide: eliminar el coche base y spawnear el nuestro
                  Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, garageVehicle, false);
                  garageVehicle.MarkAsNoLongerNeeded();
                  garageVehicle.Delete();

                  var spawnResult = VehicleSpawnManager.SpawnVehicle(data);
                  spawnResult.match<bool>(
                      onOk: spawned => {
                         insuredVehList.Add(spawned);
                         var blipResult = VehicleBlipHandler.Create(spawned);
                         if (blipResult is Ok<Blip> okBlip) blipsToRemove[data.Id] = okBlip.Value;
                         return true;
                      },
                      onErr: error => {
                         Logger.Error($"Error al restaurar vehículo {data.ModelName}: {error}");
                         return false;
                      }
                  );
               }
               // Si no coincide, no se hace nada (no eliminar, no spawnear)
            }
            model.MarkAsNoLongerNeeded();
            return;
         }

         // 3B. CASO B: Modelo NO cargado (fallback)
         model.MarkAsNoLongerNeeded();
         Vector3 fallbackPos = new Vector3(data.PosX, data.PosY, data.PosZ);
         Vehicle existingVehicle = World.GetAllVehicles()
             .FirstOrDefault(v => v.Exists()
                 && Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash) == data.ModelName
                 && v.Mods.LicensePlate == data.Plate
                 && v.Position.DistanceTo(fallbackPos) <= Constants.GARAGE_SEARCH_RADIUS);

         if (existingVehicle != null && existingVehicle.Exists()) {
            insuredVehList.Add(existingVehicle);
            var blipResult = VehicleBlipHandler.Create(existingVehicle);
            if (blipResult is Ok<Blip> okBlip) blipsToRemove[data.Id] = okBlip.Value;
         }
         else {
            Logger.Warning($"No se pudo cargar el modelo {data.ModelName} - {data.Plate}. Se mantiene el vehículo original.");
         }
      }
   }
}