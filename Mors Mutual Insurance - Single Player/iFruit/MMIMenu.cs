using System.IO;
using GTA.Native;
using MMI_SP.Config;
using NativeUI;

namespace MMI_SP.iFruit
{
    internal class MMIMenu
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private MenuPool _pool;
        private UIMenu _mainMenu;
        private UIMenu _recoverSubmenu;

        internal bool IsAnyMenuVisible =>
            (_mainMenu != null && _mainMenu.Visible) ||
            (_recoverSubmenu != null && _recoverSubmenu.Visible);

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal MMIMenu()
        {
            Reset(false);
        }

        internal void MenuPoolProcessMenus() => _pool?.ProcessMenus();

        internal void Show()
        {
            if (_mainMenu == null) return;
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        internal void Reset(bool rebuild, bool show = false)
        {
            _pool = new MenuPool();

            _mainMenu = new UIMenu("", "");
            if (File.Exists(ModSettings.BannerImage)) _mainMenu.SetBannerType(ModSettings.BannerImage);
            _pool.Add(_mainMenu);

            if (_recoverSubmenu != null) _recoverSubmenu = null;

            if (rebuild) Build();
            if (show) Show();
        }

        private void Build()
        {
            BuildRecover();
            _pool.RefreshIndex();
        }

        private void BuildRecover()
        {
            var parentItem = new UIMenuItem("Reclamar vehículo destruido", "Recupera un vehículo destruido");
            _mainMenu.AddItem(parentItem);

            _recoverSubmenu = new UIMenu("", "Selecciona vehículo a recuperar");
            if (File.Exists(ModSettings.BannerImage)) _recoverSubmenu.SetBannerType(ModSettings.BannerImage);
            _pool.Add(_recoverSubmenu);
            _mainMenu.BindMenuToItem(_recoverSubmenu, parentItem);

            MMIMenuBuilder.FillRecover(_recoverSubmenu, () => Reset(true, true));
        }
    }
}