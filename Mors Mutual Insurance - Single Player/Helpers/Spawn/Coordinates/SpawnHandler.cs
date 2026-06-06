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
         if (!success) return new None<float>();
         return new Some<float>(groundZArg.GetResult<float>());
      }

      // ==========================================
      // BLOQUE 2: Funciones
      // ==========================================

      //FixedSpawnHandler
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
         Constants.CLOSEST_NODE_TYPE, Constants.SEARCH_RADIUS, Constants.CLOSEST_NODE_FLAGS);

         if (!nodeFound)
         {
            // Fallback: usar la posición original con altura de suelo (si es posible)
            var fallbackHeight = GetGroundHeight(new Vector3(posX, posY, posZ));
            if (fallbackHeight.is_some()) {
               float groundZ = fallbackHeight.unwrap_or(Constants.GROUND_Z_FALLBACK);
               return new Ok<Vector3>(new Vector3(posX, posY, groundZ + Constants.GROUND_OFFSET));
            }
            return new Ok<Vector3>(new Vector3(posX, posY, posZ));
         }

         Vector3 nodePos = outPos.GetResult<Vector3>();

         // Ajustar altura al suelo
         var groundHeight = GetGroundHeight(nodePos);
         if (groundHeight.is_some())
         {
            float groundZ = groundHeight.unwrap_or(Constants.GROUND_Z_FALLBACK);
            if (Math.Abs(nodePos.Z - groundZ) < Constants.MAX_NODE_HEIGHT_DIFF)
               return new Ok<Vector3>(new Vector3(nodePos.X, nodePos.Y, groundZ + Constants.GROUND_OFFSET));
         }

         // Si no se pudo ajustar, devolver el nodo tal cual
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
            Constants.VEHICLE_NODE_TYPE, Constants.ROAD_NODE_RADIUS, Constants.VEHICLE_NODE_FLAGS);
         if (!success) return fallbackHeading;

         return outHeading.GetResult<float>();
      }
   }
}