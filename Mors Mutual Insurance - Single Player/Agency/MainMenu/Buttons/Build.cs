using System;
using System.IO;
using MMI_SP.Config;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using NativeUI;

namespace MMI_SP.Agency.MainMenu.Buttons
{
    internal static class Build
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<UIMenuItem> MainMenuItem(
            UIMenu parentMenu,
            string text,
            string description,
            bool enabled,
            string rightLabel,
            ItemActivatedEvent onActivated)
        {
            if (parentMenu == null) return new Err<UIMenuItem>("El menú padre es nulo.");

            UIMenuItem item = new UIMenuItem(text, description);
            item.Enabled = enabled;
            if (!string.IsNullOrEmpty(rightLabel))
                item.SetRightLabel(rightLabel);

            item.Activated += onActivated;
            parentMenu.AddItem(item);
            return new Ok<UIMenuItem>(item);
        }

        internal static Result<UIMenu> SubMenu(
            UIMenu parentMenu,
            MenuPool pool,
            string title,
            string description,
            string itemDescription,
            Action<string> onVehicleSelected,
            string emptyMessage,
            bool showDestroyed = false)
        {
            UIMenu submenu = pool.AddSubMenu(parentMenu, title, description);
            if (submenu == null) return new Err<UIMenu>("No se pudo crear el submenú.");

            if (!string.IsNullOrEmpty(itemDescription) && itemDescription != description)
            {
                UIMenuItem lastItem = parentMenu.MenuItems[parentMenu.MenuItems.Count - 1];
                lastItem.Description = itemDescription;
            }

            if (File.Exists(ModSettings.BannerImage))
                submenu.SetBannerType(ModSettings.BannerImage);

            var fillResult = Fill.SubMenu(submenu, onVehicleSelected, emptyMessage, showDestroyed);
            if (fillResult is Err<bool> err) return new Err<UIMenu>(err.Message);

            return new Ok<UIMenu>(submenu);
        }
    }
}