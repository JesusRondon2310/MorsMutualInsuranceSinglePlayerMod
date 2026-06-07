using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Config;
using MMI_SP.PatternMatching;
using System;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
   public static class SpawnHandler
   {
      // ==========================================
      // BLOQUE 1: Métodos privados
      // ==========================================

      private static Option<float> GetGroundHeight(Vector3 position)
      {
         OutputArgument groundZArg = new OutputArgument();
         bool success = Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, position.Z, groundZArg, false);
         
         if (!success) return None<float>.Instance;
         return new Some<float>(groundZArg.GetResult<float>());
      }

      // ==========================================
      // BLOQUE 2: Funciones
      // ==========================================

      public static EntityPosition GetPlayerReferencePosition() => new EntityPosition(ModSettings.PlayerPos, Constants.DEFAULT_HEADING);

      public static Result<Vector3> FixGround(Vector3 position)
      {
         return GetGroundHeight(position).match<Result<Vector3>>(
               onSome: groundZ => new Ok<Vector3>(new Vector3(position.X, position.Y, groundZ + Constants.GROUND_OFFSET)),
               onNone: () => new Ok<Vector3>(position)
         );
      }

      public static Result<Vector3> GetValidRoad(float posX, float posY, float posZ)
      {
         OutputArgument outPos = new OutputArgument();
         OutputArgument outHeading = new OutputArgument();

         bool nodeFound = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING, posX, posY, posZ, outPos, outHeading,
            Constants.CLOSEST_ROAD, Constants.VALID_ROAD_SEARCH_RADIUS, Constants.GET_CLOSEST_VALID_ROAD);

         // CASO 1: No se encontró el nodo de la carretera
         if (!nodeFound)
         {
            // ✅ APLICADA REGLA 7.9: Evaluamos y extraemos en un solo paso seguro sin unwrap_or redundantes
            if (GetGroundHeight(new Vector3(posX, posY, posZ)) is Some<float> fallbackHeight) {
               return new Ok<Vector3>(new Vector3(posX, posY, fallbackHeight.Value + Constants.GROUND_OFFSET));
            }
            return new Ok<Vector3>(new Vector3(posX, posY, posZ));
         }

         Vector3 nodePos = outPos.GetResult<Vector3>();

         // CASO 2: Nodo encontrado, intentamos ajustar la altura al suelo real
         // ✅ APLICADA REGLA 7.9: Código plano, ultra legible y libre de Hadoukens
         if (GetGroundHeight(nodePos) is Some<float> groundHeight)
         {
            if (Math.Abs(nodePos.Z - groundHeight.Value) < Constants.MAX_ROAD_HEIGHT_DIFF)
               return new Ok<Vector3>(new Vector3(nodePos.X, nodePos.Y, groundHeight.Value + Constants.GROUND_OFFSET));
         }

         // Si no se pudo ajustar la altura, devolvemos el nodo nativo tal cual
         return new Ok<Vector3>(nodePos);
      }

      public static Vector3 DriverDeliverySpawn(float distanceBehindPlayer)
      {
         Vector3 playerPos = Game.Player.Character.Position;
         return playerPos - (Game.Player.Character.ForwardVector * distanceBehindPlayer);
      }

      public static EntityPosition InstantDeliverySpawn(Vector3 playerPos)
      {
         Vector3 forward = Game.Player.Character.ForwardVector;
         Vector3 spawnPos = playerPos + (forward * Constants.DISTANCE_IN_FRONT_OF_PLAYER);
         float heading = Game.Player.Character.Heading;
         return new EntityPosition(spawnPos, heading);
      }

      public static float GetValidOrientation(Vector3 nodePos, float fallbackHeading)
      {
         OutputArgument outPos = new OutputArgument();
         OutputArgument outHeading = new OutputArgument();

         bool success = Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, nodePos.X, nodePos.Y, nodePos.Z,
            Constants.CLOSEST_VEHICLE_NODE_INDEX, outPos, outHeading,
            Constants.VEHICLE_NODE_TYPE, Constants.CLOSEST_ROAD_SEARCH_RADIUS, Constants.VEHICLE_NODE_FLAGS);
            
         if (!success) return fallbackHeading;

         return outHeading.GetResult<float>();
      }
   }
}