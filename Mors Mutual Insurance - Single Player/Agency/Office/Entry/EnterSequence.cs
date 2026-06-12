using System;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Agency.MainMenu;
using MMI_SP.PatternMatching;
using MMI_SP.Dialogue;
using MMI_SP.Debug;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class EnterSequence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<bool> Execute(UI menu, CutsceneManager cutscene, Office.Manager office)
        {
            // --- 1. Proteger el vehículo actual (si está asegurado) ---
            Vehicle currentVeh = Game.Player.Character.CurrentVehicle;
            if (currentVeh != null && currentVeh.Exists())
            {
                string vehId = VehicleIdentifier.Get(currentVeh);
                var dataOption = DB.Core.FindVehicle(vehId);
                if (dataOption.is_some())
                {
                    // Aplicar protecciones
                    VehiclePersistence.SetPersistence(currentVeh, true);
                    Logger.Debug($"Vehículo asegurado protegido al entrar a la oficina: {vehId}");
                }
            }

            Logger.Debug("Reset the menu");
            try
            {
                menu.RebuildMenu();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return new Err<bool>("Error al reconstruir el menú.");
            }

            Logger.Debug("Entering cutscene");
            cutscene.Enter();
            Cutscenes.EnteringAgency();

            Logger.Debug("Teleport the player in the office");
            Game.Player.Character.Position = Config.PlayerPos;
            Game.Player.Character.IsPositionFrozen = true;

            Logger.Debug("Force load office");
            Function.Call(Hash.LOAD_SCENE, Config.PlayerPos.X, Config.PlayerPos.Y, Config.PlayerPos.Z);
            Logger.Debug("Wait until everything is loaded");

            Screen.UIHandler(Constants.ENTRY_FADE_DURATION_MS);
            Logger.Debug("Open menu");

            try
            {
                menu.Show();
            }
            catch (Exception e)
            {
                Logger.Error("Error: EnterAgency - " + e.Message);
                return new Err<bool>("Error al mostrar el menú.");
            }

            Logger.Debug("Office creation");
            office.CreateOffice();

            Logger.Debug("_office.itemsCollection:");
            Logger.Debug("type=" + office.CurrentCollectionType);
            Logger.Debug("count=" + office.CurrentCollectionCount);

            Core.PlayRandom(Core.SpeechType.OfficeHi, Ambient.NpcHandler.CurrentNpc);
            return new Ok<bool>(true);
        }
    }
}