using GTA.Math;
using GTA.Native;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public static class GetValidOrientation
    {
        public static float FromNode(Vector3 nodePos, float fallbackHeading)
        {
            OutputArgument outPos = new OutputArgument();
            OutputArgument outHeading = new OutputArgument();

            if (Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, nodePos.X, nodePos.Y, nodePos.Z, 1, outPos, outHeading, 1, 3.0f, 0))
                return outHeading.GetResult<float>();

            return fallbackHeading;
        }
    }
}