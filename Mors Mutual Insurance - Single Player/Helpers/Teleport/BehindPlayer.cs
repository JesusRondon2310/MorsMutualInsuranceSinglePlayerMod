using GTA;
using GTA.Math;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Teleport
{
    public static class BehindPlayer
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void Execute(Vehicle veh)
        {
            float currentDistance = Game.Player.Character.Position.DistanceTo(veh.Position);
            if (currentDistance <= 200f) return;

            Vector3 candidatePos = DriverDeliverySpawn.GetPosition(150f);
            var roadResult = RoadSpawnHandler.FindNode(candidatePos.X, candidatePos.Y, candidatePos.Z);

            if (roadResult.is_ok())
            {
                Vector3 safePos = ((Ok<Vector3>)roadResult).Value;
                veh.Position = safePos;
                veh.Heading = Game.Player.Character.Heading;
            }
        }
    }
}