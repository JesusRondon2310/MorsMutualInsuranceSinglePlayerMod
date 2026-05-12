using System;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Menu
    {
        internal static UIMenuItem _itemInsure; // interno, no public

        internal static void InsureButtonBuild(UIMenu parentMenu)
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists())
            {
                RemoveExistingItem(parentMenu);
                return;
            }

            int cost = Core.GetCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = Core.IsInsured(veh);
            bool isInsurable = Core.IsInsurable(veh);

            string description;
            bool enabled;

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
            }
            else
            {
                description = $"Asegurar el último vehículo en uso\n{vehName}";
                enabled = true;
            }

            // Reemplazo limpio: eliminar el anterior, resetear referencia, crear nuevo
            RemoveExistingItem(parentMenu);

            _itemInsure = new UIMenuItem("Asegurar vehículo", description);
            _itemInsure.Enabled = enabled;
            if (enabled)
                _itemInsure.SetRightLabel(cost + "$");

            _itemInsure.Activated += Buttons.Insure;
            parentMenu.AddItem(_itemInsure);
        }

        private static void RemoveExistingItem(UIMenu parentMenu)
        {
            if (_itemInsure == null) return;

            _itemInsure.Activated -= Buttons.Insure;
            parentMenu.MenuItems.Remove(_itemInsure);
            _itemInsure = null; // ← evita la referencia fantasma
        }
    }
}