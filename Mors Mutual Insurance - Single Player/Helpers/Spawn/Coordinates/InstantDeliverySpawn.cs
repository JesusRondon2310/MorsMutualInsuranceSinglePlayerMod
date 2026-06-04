using GTA;
using GTA.Math;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public static class InstantDeliverySpawn
    {
        public static EntityPosition GetPosition(Vector3 playerPos)
        {
            Vector3 forward = Game.Player.Character.ForwardVector;
            Vector3 spawnPos = playerPos + (forward * 5.0f);
            float heading = Game.Player.Character.Heading;
            return new EntityPosition(spawnPos, heading);
        }
    }
}