using MMI_SP.Agency.MainMenu.SubMenus;
using NativeUI;

namespace MMI_SP.Agency.MainMenu
{
    internal class UI
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private MenuPool _pool;
        private UIMenu _mainMenu;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal UI()
        {
            BuildAndSubscribe();
        }

        internal void RebuildMenu()
        {
            BuildAndSubscribe();
        }

        private void BuildAndSubscribe()
        {
            _pool = new MenuPool();
            _pool.ControlDisablingEnabled = true;
            _mainMenu = new UIMenu("", "");
            _pool.Add(_mainMenu);

            ExecuteRebuild.MainMenu(_mainMenu, _pool);
        }

        internal bool IsAnyMenuVisible()
        {
            if (_mainMenu != null && _mainMenu.Visible) return true;
            if (CancelHandler.Instance?.Submenu?.Visible == true) return true;
            if (RecoverHandler.Instance?.Submenu?.Visible == true) return true;
            return false;
        }

        internal void Show() => _mainMenu.Visible = true;
        internal void Hide() => _mainMenu.Visible = false;
        internal void Update() => _pool.ProcessMenus();

        internal bool IsMainMenuVisible() => _mainMenu != null && _mainMenu.Visible;
        internal bool IsSubmenuVisible()
            => (CancelHandler.Instance?.Submenu?.Visible ?? false)
            || (RecoverHandler.Instance?.Submenu?.Visible ?? false);
    }
}