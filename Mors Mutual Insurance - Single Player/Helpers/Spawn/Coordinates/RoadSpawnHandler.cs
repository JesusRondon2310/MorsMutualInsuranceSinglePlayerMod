using GTA.Math;
using GTA.Native;
using MMI_SP.PatternMatching;
using System;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    public static class RoadSpawnHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<Vector3> FindNode(float posX, float posY, float posZ)
        {
            Vector3 originalPos = new Vector3(posX, posY, posZ);
            int[] radii = { 30, 50, 80 };
            const float maxDistance = 80f;

            foreach (int radius in radii)
            {
                OutputArgument outPos = new OutputArgument();
                OutputArgument outHeading = new OutputArgument();

                if (Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, posX, posY, posZ, 1, outPos, outHeading, 1, radius, 0))
                {
                    Vector3 nodePos = outPos.GetResult<Vector3>();

                    // Si el nodo encontrado está demasiado lejos, probamos con el siguiente radio
                    if (nodePos.DistanceTo(originalPos) > maxDistance)
                        continue;

                    OutputArgument groundZArg = new OutputArgument();
                    if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, nodePos.X, nodePos.Y, nodePos.Z, groundZArg, false))
                    {
                        float groundZ = groundZArg.GetResult<float>();
                        // Solo aceptamos el nodo si la altura coincide razonablemente con el suelo
                        if (Math.Abs(nodePos.Z - groundZ) < 3.0f)
                            return new Ok<Vector3>(new Vector3(nodePos.X, nodePos.Y, groundZ + 0.5f));
                    }
                }
            }

            // Fallback: corregir la altura de la posición original usando el suelo
            OutputArgument fallbackGroundZArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, posX, posY, posZ, fallbackGroundZArg, false))
                return new Ok<Vector3>(new Vector3(posX, posY, fallbackGroundZArg.GetResult<float>() + 0.5f));

            // Último recurso: devolver la posición original sin cambios
            return new Ok<Vector3>(new Vector3(posX, posY, posZ));
        }
    }
}