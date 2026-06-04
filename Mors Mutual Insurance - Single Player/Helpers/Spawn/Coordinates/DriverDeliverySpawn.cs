using GTA;
using GTA.Math;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public static class DriverDeliverySpawn
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Vector3 GetPosition(float distanceBehindPlayer)
        {
            Vector3 playerPos = Game.Player.Character.Position;
            return playerPos - (Game.Player.Character.ForwardVector * distanceBehindPlayer);
        }
    }
}