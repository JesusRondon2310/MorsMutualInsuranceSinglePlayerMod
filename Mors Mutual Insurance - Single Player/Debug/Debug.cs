using System.Drawing;
using MMI_SP.Insurance.Policies;
using GTA;
using MMI_SP.Helpers;   // Para acceder a Constants

namespace MMI_SP.Debug
{
    internal static class Debug
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly GTA.UI.TextElement debugTextTitle = new GTA.UI.TextElement("Last Vehicle",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehLastHandle = new GTA.UI.TextElement("Last Handle: 0",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 1 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextCurrentHandle = new GTA.UI.TextElement("Current Handle: 0",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 2 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehDriveable = new GTA.UI.TextElement("Driveable: False",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 3 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehPersistent = new GTA.UI.TextElement("Persistent: False",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 4 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehModelHash = new GTA.UI.TextElement("Modelhash: False",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 5 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehGameplayCamera = new GTA.UI.TextElement("GameplayCamera: False",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 6 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehInsured = new GTA.UI.TextElement("Insured: False",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 7 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);
        private static readonly GTA.UI.TextElement debugTextVehPrice = new GTA.UI.TextElement("Price: 0",
            new PointF(Constants.DEBUG_TEXT_START_X, Constants.DEBUG_TEXT_START_Y + 8 * Constants.DEBUG_TEXT_LINE_HEIGHT), Constants.DEBUG_TEXT_SIZE);

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static void ShowVehicleInfo(Vehicle veh)
        {
            Vehicle current = Game.Player.Character.CurrentVehicle;
            if (veh != null)
            {
                debugTextTitle.Draw();
                debugTextVehLastHandle.Caption = "Last Handle: " + veh.Handle.ToString();
                debugTextVehLastHandle.Draw();
                if (current != null)
                {
                    debugTextCurrentHandle.Caption = "Current Handle: " + current.Handle.ToString();
                    debugTextCurrentHandle.Draw();
                }
                debugTextVehDriveable.Caption = "Driveable: " + veh.IsDriveable.ToString();
                debugTextVehDriveable.Draw();
                debugTextVehPersistent.Caption = "Persistent: " + veh.IsPersistent.ToString();
                debugTextVehPersistent.Draw();
                debugTextVehModelHash.Caption = "ModelHash: " + veh.Model.Hash.ToString();
                debugTextVehModelHash.Draw();
                debugTextVehGameplayCamera.Caption = "GameplayCamera: " + GameplayCamera.IsRendering;
                debugTextVehGameplayCamera.Draw();
                debugTextVehInsured.Caption = "Insured: " + Manager.IsInsured(veh).ToString();
                debugTextVehInsured.Draw();
                debugTextVehPrice.Caption = "Price: " + Manager.GetCost(veh).ToString();
                debugTextVehPrice.Draw();
            }
        }
    }
}