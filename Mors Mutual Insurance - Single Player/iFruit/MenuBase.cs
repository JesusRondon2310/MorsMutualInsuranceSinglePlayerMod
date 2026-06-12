using System.IO;
using GTA.Native;
using NativeUI;

namespace MMI_SP.iFruit
{
    internal abstract class MenuBase
    {
        protected MenuPool _pool;
        protected UIMenu _mainMenu;
        protected string _bannerPath;
        protected string _title;
        protected string _subtitle;

        protected MenuBase(string title, string subtitle, string bannerPath = null)
        {
            _title = title;
            _subtitle = subtitle;
            _bannerPath = bannerPath;
            Reset(true, false);
        }

        public virtual bool IsAnyMenuVisible => _mainMenu?.Visible == true;

        public virtual void Show()
        {
            if (_mainMenu == null) return;
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        public virtual void Hide()
        {
            if (_mainMenu != null)
                _mainMenu.Visible = false;
        }

        public virtual void MenuPoolProcessMenus() => _pool?.ProcessMenus();

        public virtual void Reset(bool rebuild, bool show = false)
        {
            _pool = new MenuPool();
            _mainMenu = new UIMenu(_title, _subtitle);
            if (!string.IsNullOrEmpty(_bannerPath) && File.Exists(_bannerPath))
                _mainMenu.SetBannerType(_bannerPath);
            _pool.Add(_mainMenu);

            if (rebuild)
                Build();

            if (show)
                Show();
        }

        protected abstract void Build();

        // Método auxiliar para crear submenús con el mismo banner
        protected UIMenu CreateSubmenu(string title, string subtitle, UIMenuItem parentItem)
        {
            var sub = new UIMenu(title, subtitle);
            if (!string.IsNullOrEmpty(_bannerPath) && File.Exists(_bannerPath))
                sub.SetBannerType(_bannerPath);
            _pool.Add(sub);
            _mainMenu.BindMenuToItem(sub, parentItem);
            return sub;
        }
    }
}