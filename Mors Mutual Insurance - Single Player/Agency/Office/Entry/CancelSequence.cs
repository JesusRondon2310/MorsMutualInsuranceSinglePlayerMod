using System;
using GTA;
using MMI_SP.Agency.MainMenu;
using MMI_SP.Agency.Office.Ambient;
using MMI_SP.Dialogue;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class CancelSequence
    {
        public static Result<bool> Execute(UI menu, CutsceneManager cutscene, bool showMenu)
        {
            cutscene.Exit();
            Game.Player.Character.Position = Reception.Position;
            Game.Player.Character.IsPositionFrozen = false;
            GTA.UI.Screen.FadeIn(Constants.ENTRY_FADE_DURATION_MS);
            ScriptCameraDirector.StopRendering();

            if (showMenu)
            {
                menu.RebuildMenu();
                menu.Show();
            }

            Core.PlayRandom(Core.SpeechType.OfficeBye, NpcHandler.CurrentNpc);

            return new Ok<bool>(true);
        }
    }
}