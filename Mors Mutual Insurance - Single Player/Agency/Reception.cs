using System;
using GTA;
using GTA.Math;
using MMI_SP.Agency.Office.Entry;
using MMI_SP.Debug;
using MMI_SP.Helpers;

namespace MMI_SP.Agency
{
    class Reception : Script
    {
        private Blip _agencyBlip;
        private static Vector3 _position = new Vector3(Constants.AGENCY_POS_X, Constants.AGENCY_POS_Y, Constants.AGENCY_POS_Z);
        public static Vector3 Position => _position;

        private Handler _state;

        public Reception()
        {
            Tick += Initialize;
        }

        void Initialize(object sender, EventArgs e)
        {
            _agencyBlip = CreateBlip();
            _state = new Handler(this);

            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        void OnTick(object sender, EventArgs e)
        {
            _state?.OnTick();
            DisplayAgencyThisFrame();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _state?.CleanUp();
            if (_agencyBlip.Exists()) _agencyBlip.Delete();
        }

        private void DisplayAgencyThisFrame()
        {
            if (Game.Player.Character.Position.DistanceTo(_position) >= Constants.RECEPTION_DISTANCE) return;
            if (Game.Player.Character.IsInVehicle()) return;

            if (Game.Player.Wanted.WantedLevel > Constants.ZERO)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("No puedes entrar con nivel de búsqueda.");
                return;
            }

            string buttonPrompt = InputHandler.GetEnterOfficePrompt();
            GTA.UI.Screen.ShowHelpTextThisFrame($"Presiona {buttonPrompt} para entrar a la oficina de Mors Mutual.");

            if (!InputHandler.IsEnterOfficePressed()) return;

            try
            {
                Logger.Debug("Reception: Intentando entrar...");
                _state.Enter();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                GTA.UI.Notification.PostTicker("MMI-SP: Error al crear la oficina.", false);
                _state.ErrorCancel(true);
            }
        }

        private Blip CreateBlip()
        {
            Blip blip = World.CreateBlip(_position);
            blip.Sprite = BlipSprite.Michael;
            blip.Color = (BlipColor)Constants.AGENCY_BLIP_COLOR;
            blip.Name = "Mors Mutual Insurance";
            blip.IsShortRange = true;
            return blip;
        }
    }
}