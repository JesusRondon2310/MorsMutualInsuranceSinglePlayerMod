using GTA;
using GTA.Input;
using System.Windows.Forms;

namespace MMI_SP.Helpers
{
    public static class InputHandler
    {
        // ==========================================
        // BLOQUE 1: Detección del dispositivo activo
        // ==========================================
        public static bool IsUsingController => Game.LastInputMethod == InputMethod.GamePad;

        // ==========================================
        // BLOQUE 2: Verificación de pulsaciones
        // ==========================================
        public static bool IsLockVehiclePressed()
        {
            // Control.FrontendRight es el botón de aceptar/avanzar (E en teclado, A o X en mando)
            return Controls.IsDisabledControlJustPressed(ControlType.PlayerControl, (ControlAction)GTA.Control.FrontendRight)
                || Game.IsKeyPressed(Keys.L);
        }

        public static bool IsEnterOfficePressed()
        {
            return Controls.IsDisabledControlJustPressed(ControlType.PlayerControl, (ControlAction)GTA.Control.FrontendRight)
                || Game.IsKeyPressed(Keys.E);
        }

        // ==========================================
        // BLOQUE 3: Mensajes de ayuda dinámicos
        // ==========================================
        public static string GetLockVehiclePrompt()
        {
            return IsUsingController
                ? "~INPUT_FRONTEND_RIGHT~"
                : "~y~L~w~";
        }

        public static string GetEnterOfficePrompt()
        {
            return IsUsingController
                ? "~INPUT_FRONTEND_RIGHT~"
                : "~y~E~w~";
        }
    }
}