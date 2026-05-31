using GTA;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class CleanUp
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<bool> Execute()
        {
            ScriptCameraDirector.StopRendering();

            if (Builder.CurrentCamera != null)
            {
                Builder.CurrentCamera.IsActive = false;
                Builder.CurrentCamera.Delete();
            }

            World.Weather = Builder.CurrentWeather;

            if (Builder.CurrentNpc != null && Builder.CurrentNpc.Exists())
            {
                Builder.CurrentNpc.Task.ClearAllImmediately();
                Builder.CurrentNpc.Delete();
            }

            return new Ok<bool>(true);
        }
    }
}