using GTA;
using MMI_SP.Config;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Drawing;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleChangeHandler
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Vehicle _previousVehicle = null;
        private static int _insuranceIconTimer = 0;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Handle(
            List<Vehicle> insuredVehList,
            List<Vehicle> recoveredVehList,
            Dictionary<string, Blip> blipsToRemove)
        {
            HandleVehicleChange(blipsToRemove);
            DrawInsuranceSprite();
        }

        private static void HandleVehicleChange(Dictionary<string, Blip> blipsToRemove)
        {
            if (_previousVehicle == Game.Player.Character.CurrentVehicle) return;

            Vehicle previous = _previousVehicle;
            _previousVehicle = Game.Player.Character.CurrentVehicle;

            if (_previousVehicle != null)
            {
                RecoveryBlipHandler.RemoveBlip(_previousVehicle, blipsToRemove);
                _insuranceIconTimer = Game.GameTime + 4270;

                if (Insurer.Instance.IsInsured(_previousVehicle))
                    Insurer.Instance.UpdateVehicleData(_previousVehicle).match<bool>(
                        onOk: _ => true,
                        onErr: error =>
                        {
                            Logger.Error($"Error al guardar modificaciones al subir al vehículo: {error}");
                            return false;
                        }
                    );
            }
            else if (previous != null && previous.Exists())
            {
                if (Insurer.Instance.IsInsured(previous))
                {
                    RecoveryBlipHandler.RestoreBlip(previous, blipsToRemove);
                    Insurer.Instance.UpdateVehicleData(previous).match<bool>(
                        onOk: _ => true,
                        onErr: error =>
                        {
                            Logger.Error($"Error al guardar posición de aparcamiento: {error}");
                            return false;
                        }
                    );
                }
            }
        }

        private static void DrawInsuranceSprite()
        {
            if (Game.GameTime >= _insuranceIconTimer) return;

            Vehicle currentVeh = Game.Player.Character.CurrentVehicle;
            if (currentVeh == null || !Insurer.IsInsurable(currentVeh)) return;

            bool isInsured = Insurer.Instance.IsInsured(currentVeh);
            Color color = isInsured ? Color.FromArgb(35, 199, 128) : Color.FromArgb(190, 0, 50);
            Sprite.InsuranceStatus(ModSettings.InsuranceImage, 1225f, 600f, color);
        }
    }
}