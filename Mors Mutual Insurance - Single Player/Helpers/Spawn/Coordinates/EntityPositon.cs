using GTA.Math;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public struct EntityPosition
    {
        public Vector3 Position;
        public float Heading;

        public EntityPosition(Vector3 pos, float heading)
        {
            Position = pos;
            Heading = heading;
        }
    }
}