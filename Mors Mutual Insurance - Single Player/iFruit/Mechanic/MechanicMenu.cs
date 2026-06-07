using NativeUI;
using MMI_SP.Config;
using MMI_SP.Helpers;

namespace MMI_SP.iFruit.Mechanic
{
    internal class MechanicMenu : MenuBase
    {
        private static readonly string MechanicBanner = ModSettings.BaseDir + Constants.DEFAULT_MECHANIC_BANNER_IMAGE_EXPR;
        private UIMenu _bringSubmenu;

        public override bool IsAnyMenuVisible => base.IsAnyMenuVisible || (_bringSubmenu?.Visible == true);

        public MechanicMenu() : base("Mecánico", "Solicitar vehículo", MechanicBanner) { }

        protected override void Build()
        {
            BuildBring();
        }

        private void BuildBring()
        {
            var parentItem = new UIMenuItem("Solicitar vehículo", "El Mecánico te llevará el vehículo");
            _mainMenu.AddItem(parentItem);
            _bringSubmenu = CreateSubmenu("", "Selecciona un vehículo", parentItem);
            MechanicMenuBuilder.FillBring(_bringSubmenu, () => Reset(true, true));
        }

        public override void Reset(bool rebuild, bool show = false)
        {
            _bringSubmenu = null;
            base.Reset(rebuild, show);
        }
    }
}