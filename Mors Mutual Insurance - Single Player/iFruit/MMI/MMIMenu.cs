using MMI_SP.Config;
using NativeUI;

namespace MMI_SP.iFruit.MMI
{
    internal class MMIMenu : MenuBase
    {
        private UIMenu _recoverSubmenu;

        public override bool IsAnyMenuVisible => base.IsAnyMenuVisible || (_recoverSubmenu?.Visible == true);

        public MMIMenu() : base("", "", ModSettings.BannerImage) { }

        protected override void Build()
        {
            BuildRecover();
        }

        private void BuildRecover()
        {
            var parentItem = new UIMenuItem("Reclamar vehículo destruido", "Recupera un vehículo destruido");
            _mainMenu.AddItem(parentItem);
            _recoverSubmenu = CreateSubmenu("", "Selecciona vehículo a recuperar", parentItem);
            MMIMenuBuilder.FillRecover(_recoverSubmenu, () => Reset(true, true));
        }

        public override void Reset(bool rebuild, bool show = false)
        {
            _recoverSubmenu = null;
            base.Reset(rebuild, show);
        }
    }
}