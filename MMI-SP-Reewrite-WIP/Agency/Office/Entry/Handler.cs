using GTA;
using MMI_SP.Agency.MainMenu;
using System;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Entry
{
    internal class Handler
    {
        // ==========================================
        // BLOQUE 1: Variables y creación del objeto
        // ==========================================
        private readonly Office.Manager _office;
        private readonly CutsceneManager _cutscene;
        private readonly UI _menu;
        private bool _isExiting = false;
        private bool _isInside = false;

        public Handler(Script script)
        {
            _office = new Office.Manager();
            _cutscene = new CutsceneManager();
            _menu = new UI();
        }

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public void Enter()
        {
            _isExiting = false;
            _isInside = true;

            var result = EnterSequence.Execute(_menu, _cutscene, _office);

            result.Match<bool>(
                onOk: _ => true, // Todo bien, seguimos
                onErr: error =>
                {
                    // Si falla la secuencia de entrada, cancelamos y notificamos
                    GTA.UI.Notification.PostTicker("MMI-SP: " + error, true, false);
                    ErrorCancel(menu: true);
                    return false;
                }
            );
        }

        public void Exit()
        {
            _isInside = false;
            ExitSequence.Execute(_office, _cutscene);
        }

        public void ErrorCancel(bool menu = true)
        {
            _isInside = false;
            CancelSequence.Execute(_menu, _cutscene, menu);
        }

        public void OnTick()
        {
            _cutscene.UpdateHUD();
            _office.UpdateSpeechTimer();
            _menu?.Update();

            if (_isInside && _menu != null && !_menu.IsAnyMenuVisible() && !_isExiting)
            {
                Logger.Debug($"MainMenu visible: {_menu.IsMainMenuVisible()}, Submenu visible: {_menu.IsSubmenuVisible()}");
                _isExiting = true;
                Exit();
            }
        }

        public void CleanUp()
        {
            CleanUpSequence.Execute(_office);
        }
    }
}