using GTA.Math;
using GTA.Native;
using MMI_SP.Config;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public static class FixedSpawnHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static EntityPosition GetRecoverNode()
        {
            return new EntityPosition(ModSettings.PlayerPos, 0.0f);
        }

        public static Result<Vector3> FixGround(Vector3 position)
        {
            OutputArgument groundZArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, position.Z, groundZArg, false))
                return new Ok<Vector3>(new Vector3(position.X, position.Y, groundZArg.GetResult<float>() + 0.5f));

            return new Ok<Vector3>(position);
        }
    }
}