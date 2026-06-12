using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;

namespace MMI_SP.Helpers
{
   public static class VehiclesInGarage
   {
      // ==========================================
      // BLOQUE 1: Datos
      // ==========================================
      public static readonly List<string> GarageNames = new List<string>
      {
         "Trevor - Sandy Shores",
         "Trevor - Vespucci",
         "Trevor - Pillbox Hill",
         "Michael - Beverly Hills",
         "Michael - Blaine County",
         "Michael - Los Santos",
         "Franklin - Aunt",
         "Franklin - Grove Street",
         "Franklin - Hills",
         "Franklin - Downtown",
      };

      // Coordenadas de los garajes comprables (interiores de personajes) - se mantiene lista por la coordenada Z
      public static readonly List<Vector3> GaragePosition = new List<Vector3> { new Vector3(198.85f, -1019.01f, -99.0f) };

      public static readonly List<Vector3> InsideSlots = new List<Vector3> {
            new Vector3(192.788452f, -1020.57056f, -99.46524f), // 1
            new Vector3(196.279678f, -1020.51538f, -99.60878f), // 2
            new Vector3(199.890869f, -1020.04382f, -99.65353f), // 3
            new Vector3(203.59726f,  -1019.78491f, -99.68882f)  // 4
        };

      // Depósitos policiales
      public static readonly List<Vector3> PoliceImpoundPositions = new List<Vector3>
        {
            new Vector3(433.848f, -996.551f, 25.771f) // Los Santos
        };

      // ==========================================
      // BLOQUE 2: Funciones privadas
      // ==========================================
      private static bool IsWithinRadius(Vector3 position, Vector3 center, float radius)
      {
         Vector2 pos2D = new Vector2(position.X, position.Y);
         Vector2 center2D = new Vector2(center.X, center.Y);
         return Vector2.Distance(pos2D, center2D) <= radius;
      }

      // ==========================================
      // BLOQUE 3: Funciones públicas
      // ==========================================
      public static bool IsDefaultGarage(Vehicle veh)
      {
         if (veh == null || !veh.Exists()) return false;
         foreach (string garageName in GarageNames) if (Function.Call<bool>(Hash.IS_VEHICLE_IN_GARAGE_AREA, garageName, veh)) return true;
         return false;
      }

      // Detecta si el vehículo está en un garaje comprable (por coordenadas, ignorando Z)
      public static bool IsAnInteriorGarage(Vehicle veh)
      {
         if (veh == null || !veh.Exists()) return false;
         foreach (Vector3 garagePos in GaragePosition) if (IsWithinRadius(veh.Position, garagePos, Constants.INTERIOR_GARAGE_RADIUS)) return true;
         return false;
      }

      // Detecta si una posición está dentro del garaje interior (útil para salida a pie)
      public static bool IsPositionInInteriorGarage(Vector3 position)
      {
         if (GaragePosition.Count == 0) return false;
         return IsWithinRadius(position, GaragePosition[Constants.FIRST_INDEX], Constants.INTERIOR_GARAGE_RADIUS);
      }

      public static bool IsPositionInPoliceImpound(Vector3 position)
      {
         if (PoliceImpoundPositions.Count == 0) return false;
         return IsWithinRadius(position, PoliceImpoundPositions[Constants.FIRST_INDEX], Constants.LS_POLICE_IMPOUND_RADIUS);
      }
   }
}