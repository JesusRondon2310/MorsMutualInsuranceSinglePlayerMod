using GTA;
using MMI_SP.Helpers.Spawn.Coordinates;

namespace MMI_SP.Helpers.Teleport
{
    public static class InFrontOfPlayer
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void Execute(Vehicle veh)
        {
            EntityPosition pos = InstantDeliverySpawn.GetPosition(Game.Player.Character.Position);
            veh.Position = pos.Position;
            veh.Heading = pos.Heading;
        }
    }
}