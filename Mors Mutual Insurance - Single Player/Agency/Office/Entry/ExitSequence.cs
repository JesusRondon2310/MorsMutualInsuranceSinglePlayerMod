using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using MMI_SP.Agency.Office.Ambient;
using MMI_SP.Debug;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class ExitSequence
    {
        public static Result<bool> Execute(Office.Manager office, CutsceneManager cutscene)
        {
            // 1. Capturar el vehículo actual ANTES de la transición
            Vehicle currentVeh = Game.Player.Character.CurrentVehicle;
            bool isInsured = false;
            if (currentVeh != null && currentVeh.Exists())
            {
                string vehId = VehicleIdentifier.Get(currentVeh);
                var dataOption = DB.Core.FindVehicle(vehId);
                if (dataOption.is_some())
                {
                    isInsured = true;
                    Logger.Debug($"Vehículo asegurado detectado al salir: {vehId}");
                }
            }

            // 2. Transición de salida (código original)
            GTA.UI.Screen.FadeOut(1000);
            Screen.UIHandler(1000);

            office.DestroyOffice();

            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.Position = Reception.Position;

            Function.Call(Hash.LOAD_SCENE, Reception.Position.X, Reception.Position.Y, Reception.Position.Z);
            Screen.UIHandler(1000);

            Cutscenes.LeavingAgency();
            cutscene.Exit();
            Dialogue.Core.PlayRandom(Dialogue.Core.SpeechType.OfficeBye, NpcHandler.CurrentNpc);

            // 3. Revertir protecciones (si el vehículo sigue existiendo y estaba asegurado)
            if (isInsured && currentVeh != null && currentVeh.Exists())
            {
                VehiclePersistence.SetPersistence(currentVeh, false);
                //Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, currentVeh, true);
                Logger.Debug($"Protecciones revertidas para el vehículo: {VehicleIdentifier.Get(currentVeh)}");
            }
            else if (isInsured)
            {
                Logger.Warning("El vehículo asegurado ya no existe al salir de la oficina. Se confía en el Initializer.");
            }

            GTA.UI.Screen.FadeIn(1000);
            return new Ok<bool>(true);
        }
    }
}