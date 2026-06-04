using GTA;
using GTA.Native;
using GTA.Math;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency
{
    internal static class Cutscenes
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<bool> EnteringAgency()
        {
            Vector3 agencyPosition = new Vector3(-825.7242f, -261.2752f, 37.0000f);
            Vector3 entranceCameraPos = new Vector3(-826.7672f, -255.3226f, 40.54334f);
            Vector3 entranceCameraTarget = new Vector3(-825.814f, -265.1871f, 37.62714f);
            Vector3 entrancePlayerPos = new Vector3(-822.528f, -260.00f, 35.79341f);
            Vector3 playerTarget = agencyPosition;
            playerTarget.Z = 40.0f;
            float playerHeading = 130.3831f;
            int walkDuration = 2500;
#pragma warning disable CS0618
            Camera cam = World.CreateCamera(entranceCameraPos, new Vector3(0.0f, 0.0f, 0.0f), GameplayCamera.FieldOfView);
#pragma warning restore CS0618
            if (cam == null) return new Err<bool>("No se pudo crear la cámara de la cinemática.");

            cam.PointAt(entranceCameraTarget);

            Game.Player.Character.Weapons.Select(WeaponHash.Unarmed, true);
            Game.Player.Character.Position = entrancePlayerPos;
            Game.Player.Character.Heading = playerHeading;
            Game.Player.Character.Task.LookAt(playerTarget, walkDuration);
            Function.Call(Hash.SIMULATE_PLAYER_INPUT_GAIT, Game.Player, 1.0f, walkDuration, 1.0f, 1, 0);
#pragma warning disable CS0618
            World.RenderingCamera = cam;
#pragma warning restore CS0618
            Helpers.Screen.UIHandler(walkDuration - 1000);

            GTA.UI.Screen.FadeOut(1000);
            Helpers.Screen.UIHandler(1000);

            ScriptCameraDirector.StopRendering();
            cam.IsActive = false;
            cam.Delete();

            return new Ok<bool>(true);
        }

        public static Result<bool> LeavingAgency()
        {
            float playerHeading = 305.54f;
            int walkDuration = 2000;

            Game.Player.Character.Heading = playerHeading;
            GameplayCamera.RelativeHeading = 0.0f;
            GameplayCamera.RelativePitch = 0.0f;

            Function.Call(Hash.SIMULATE_PLAYER_INPUT_GAIT, Game.Player, 1.0f, walkDuration, 1.0f, 1, 0);

            GTA.UI.Screen.FadeIn(1000);

            return new Ok<bool>(true);
        }
    }
}