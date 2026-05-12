using System;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Agency.MainMenu;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class EnterSequence
    {
        public static void Execute(UI menu, CutsceneManager cutscene, Office.Manager office)
        {
            Logger.Debug("EnterSequence: Iniciando secuencia de entrada...");
            // El menú se reconstruye fuera, en Manager.Enter()
            try
            {
                cutscene.Enter();
                Cutscenes.EnteringAgency();

                Game.Player.Character.Position = Config.PlayerPos;
                Game.Player.Character.IsPositionFrozen = true;

                Function.Call(Hash.LOAD_SCENE, Config.PlayerPos.X,
                              Config.PlayerPos.Y, Config.PlayerPos.Z);
                Screen.UIHandler(1000);

                menu.Show();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                GTA.UI.Notification.Show("MMI-SP: Error with module NativeUI!");
                CancelSequence.Execute(menu, cutscene, null, false);
                return;
            }

            office.CreateOffice();
            office.StartSpeechTimer();
        }
    }
}