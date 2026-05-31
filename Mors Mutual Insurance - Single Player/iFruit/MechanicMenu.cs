using System.IO;
using GTA.Native;
using NativeUI;
using MMI_SP.Config;

namespace MMI_SP.iFruit
{
    internal class MechanicMenu
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly string MechanicBanner = ModSettings.BaseDir + "\\mechanic_banner.png";

        private MenuPool _pool;
        private UIMenu _mainMenu;
        private UIMenu _bringSubmenu;

        internal bool IsAnyMenuVisible =>
            (_mainMenu != null && _mainMenu.Visible) ||
            (_bringSubmenu != null && _bringSubmenu.Visible);

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal MechanicMenu()
        {
            _pool = new MenuPool();
            _mainMenu = new UIMenu("Mecánico", "Solicitar vehículo");
            if (File.Exists(MechanicBanner))
                _mainMenu.SetBannerType(MechanicBanner);
            _pool.Add(_mainMenu);
        }

        internal void MenuPoolProcessMenus() => _pool.ProcessMenus();

        internal void Show()
        {
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        internal void Hide()
        {
            _mainMenu.Visible = false;
        }

        internal void Reset(bool rebuild, bool show = false)
        {
            _pool = new MenuPool();

            _mainMenu = new UIMenu("Mecánico", "Solicitar vehículo");
            if (File.Exists(MechanicBanner)) _mainMenu.SetBannerType(MechanicBanner);
            _pool.Add(_mainMenu);

            if (_bringSubmenu != null) _bringSubmenu = null;

            if (rebuild) Build();
            if (show) Show();
        }

        private void Build()
        {
            BuildBring();
            _pool.RefreshIndex();
        }

        private void BuildBring()
        {
            var parentItem = new UIMenuItem("Solicitar vehículo", "El Mecánico te llevará el vehículo");
            _mainMenu.AddItem(parentItem);

            _bringSubmenu = new UIMenu("", "Selecciona un vehículo");
            if (File.Exists(MechanicBanner)) _bringSubmenu.SetBannerType(MechanicBanner);
            _pool.Add(_bringSubmenu);
            _mainMenu.BindMenuToItem(_bringSubmenu, parentItem);

            MechanicMenuBuilder.FillBring(_bringSubmenu, () =>
            {
                Reset(true);
                Hide();
            });
        }
    }
}