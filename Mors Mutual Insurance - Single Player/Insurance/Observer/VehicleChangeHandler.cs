using GTA;
using MMI_SP.Config;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
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
        private static int _insuranceIconTimer = Constants.ZERO;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Handle(Dictionary<string, Blip> blipsToRemove)
        {
            HandleVehicleChange(blipsToRemove);
            DrawInsuranceSprite();
        }

        private static void UpdateVehicleDataIfInsured(Vehicle veh)
        {
            if (!Policies.Manager.IsInsured(veh)) return;

            Policies.Manager.UpdateVehicleData(veh).match<bool>(
                onOk: _ => true,
                onErr: error =>
                {
                    Logger.Error($"Error al guardar datos del vehículo: {error}");
                    return false;
                }
            );
        }

        private static void HandleVehicleChange(Dictionary<string, Blip> blipsToRemove)
        {
            if (_previousVehicle == Game.Player.Character.CurrentVehicle) return;

            Vehicle previous = _previousVehicle;
            _previousVehicle = Game.Player.Character.CurrentVehicle;

            if (_previousVehicle != null)
            {
                RecoveryBlipHandler.RemoveBlip(_previousVehicle, blipsToRemove);
                _insuranceIconTimer = Game.GameTime + Constants.INSURANCE_ICON_DISPLAY_MS;
                UpdateVehicleDataIfInsured(_previousVehicle);
            }
            else if (previous != null && previous.Exists())
            {
                if (Policies.Manager.IsInsured(previous))
                {
                    RecoveryBlipHandler.RestoreBlip(previous, blipsToRemove);
                    UpdateVehicleDataIfInsured(previous);
                }
            }
        }

        private static void DrawInsuranceSprite()
        {
            if (Game.GameTime >= _insuranceIconTimer) return;

            Vehicle currentVeh = Game.Player.Character.CurrentVehicle;
            if (currentVeh == null || !Policies.Manager.IsInsurable(currentVeh)) return;

            bool isInsured = Policies.Manager.IsInsured(currentVeh);
            Color color = isInsured ? Constants.INSURED_COLOR : Constants.NOT_INSURED_COLOR;
            Sprite.InsuranceStatus(ModSettings.InsuranceImage, Constants.SPRITE_INSURANCE_X, Constants.SPRITE_INSURANCE_Y, color);
        }
    }
}