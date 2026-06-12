using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using MMI_SP.Helpers;

namespace MMI_SP.Agency.MainMenu.Buttons
{
    internal static class Fill
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<bool> SubMenu(UIMenu submenu, Action<string> onVehicleSelected, string emptyMessage, bool showDestroyed = false)
        {
            if (submenu == null) return new Err<bool>("El submenú es nulo.");
            submenu.Clear();

            List<string> items = showDestroyed ? Manager.GetDestroyedList() : Manager.GetInsuredList();

            if (items == null) return new Err<bool>("La lista de vehículos es nula.");

            if (items.Count == Constants.NONE)
            {
                UIMenuItem emptyItem = new UIMenuItem("Vacío", emptyMessage);
                emptyItem.Enabled = false;
                submenu.AddItem(emptyItem);
                return new Ok<bool>(true);
            }

            foreach (string vehId in items)
            {
                string modelName = GetModelName(vehId);
                string plate = GetPlate(vehId);
                UIMenuItem item = new UIMenuItem(modelName, $"Modelo: ~b~{modelName}~w~ - ~y~{plate}~w~");
                item.Activated += (sender, selectedItem) => onVehicleSelected(vehId);
                submenu.AddItem(item);
            }

            return new Ok<bool>(true);
        }

        private static string GetModelName(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length >= Constants.ONE && int.TryParse(parts[Constants.FIRST_INDEX], out int modelHash))
            {
                string name = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, modelHash);
                if (!string.IsNullOrEmpty(name))
                {
                    string localized = Game.GetLocalizedString(name);
                    if (!string.IsNullOrEmpty(localized)) return localized;
                    return name;
                }
            }
            return "Desconocido";
        }

        private static string GetPlate(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            return parts.Length >= Constants.MIN_PARTS_FOR_PLATE ? parts[Constants.PLATE_INDEX] : "";
        }
    }
}