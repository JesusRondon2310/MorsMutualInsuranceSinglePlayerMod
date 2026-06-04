using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.PatternMatching;
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
            "Franklin - Vinewood Hills",
            "Franklin - Downtown",
        };

        // Coordenadas de los garajes comprables (interiores de personajes)
        public static readonly List<Vector3> GaragePosition = new List<Vector3> { new Vector3(198.85f, -1019.01f, -99.0f) };

        public static readonly List<Vector3> InsideSlots = new List<Vector3> {
            new Vector3(196.279678f, -1020.51538f, -99.60878f), // Sultan
            new Vector3(192.788452f, -1020.57056f, -99.46524f), // Dominator
            new Vector3(199.890869f, -1020.04382f, -99.65353f), // er34
            new Vector3(203.59726f,  -1019.78491f, -99.68882f)  // Kanjo
        };

        public const float GarageRadius = 20f; // Radio suficiente para cubrir el área de aparcamiento

        // Depósitos policiales
        public static readonly List<Vector3> PoliceImpoundPositions = new List<Vector3>
        {
            new Vector3(433.848f, -996.551f, 25.771f) ,   // Los Santos (Rancho/Davis)
        };
        public const float PoliceImpoundRadius = 11f;

        // ==========================================
        // BLOQUE 2: Funciones
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
            Vector2 pos2D = new Vector2(veh.Position.X, veh.Position.Y);
            foreach (Vector3 garagePos in GaragePosition)
            {
                Vector2 garagePos2D = new Vector2(garagePos.X, garagePos.Y);
                if (Vector2.Distance(pos2D, garagePos2D) <= GarageRadius) return true;
            }
            return false;
        }

        // NUEVO: Detecta si una posición está dentro del garaje interior (útil para salida a pie)
        public static bool IsPositionInInteriorGarage(Vector3 position)
        {
            Vector2 pos2D = new Vector2(position.X, position.Y);
            Vector2 center2D = new Vector2(GaragePosition[0].X, GaragePosition[0].Y);
            return Vector2.Distance(pos2D, center2D) <= GarageRadius;
        }

        public static bool IsPositionInPoliceImpound(Vector3 position)
        {
            foreach (var impoundPos in PoliceImpoundPositions) if (position.DistanceTo(impoundPos) <= PoliceImpoundRadius) return true;
            return false;
        }
    }
}