using System;
using NativeUI;

namespace MMI_SP.Agency.MainMenu
{
    internal class UI
    {
        private MenuPool _pool;
        private UIMenu _mainMenu;
        private Action _onCloseAction;
        private bool _closeSubscribed;

        private void HandleOnMenuClose(UIMenu sender) => _onCloseAction?.Invoke();

        internal UI(Action onClose)
        {
            BuildAndSubscribe(onClose);
        }

        public void RebuildMenu(Action onClose)
        {
            if (_mainMenu != null && _closeSubscribed)
            {
                _mainMenu.OnMenuClose -= HandleOnMenuClose;
                _closeSubscribed = false;
            }

            BuildAndSubscribe(onClose);
        }

        private void BuildAndSubscribe(Action onClose)
        {
            (_pool, _mainMenu) = Interface.Build();
            _pool.RefreshIndex();
            Execute.Rebuild(_mainMenu);

            _onCloseAction = onClose;
            if (!_closeSubscribed)
            {
                _mainMenu.OnMenuClose += HandleOnMenuClose;
                _closeSubscribed = true;
            }
        }

        internal void Show() => _mainMenu.Visible = true;
        internal void Hide() => _mainMenu.Visible = false;
        internal void Update() => _pool.ProcessMenus();
    }
}