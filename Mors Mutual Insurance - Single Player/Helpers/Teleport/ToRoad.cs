using GTA;
using GTA.Math;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Teleport
{
    public static class ToRoad
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void Execute(Vehicle veh)
        {
            var result = RoadSpawnHandler.FindNode(veh.Position.X, veh.Position.Y, veh.Position.Z);
            if (result.is_ok())
                veh.Position = ((Ok<Vector3>)result).Value;
        }
    }
}