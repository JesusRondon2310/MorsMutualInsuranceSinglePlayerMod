using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using MMI_SP.Agency.Office.Ambient;
using MMI_SP.Dialogue;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class ExitSequence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<bool> Execute(Office.Manager office, CutsceneManager cutscene)
        {
            GTA.UI.Screen.FadeOut(1000);
            Screen.UIHandler(1000);

            office.DestroyOffice();

            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.Position = Reception.Position;

            Function.Call(Hash.LOAD_SCENE, Reception.Position.X,
                          Reception.Position.Y, Reception.Position.Z);
            Screen.UIHandler(1000);

            Cutscenes.LeavingAgency();
            cutscene.Exit();
            Core.PlayRandom(Core.SpeechType.OfficeBye, NpcHandler.CurrentNpc);

            return new Ok<bool>(true);
        }
    }
}