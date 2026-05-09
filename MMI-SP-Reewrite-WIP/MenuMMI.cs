using GTA;
using GTA.Native;
using MMI_SP.Common;
using NativeUI;
using System;
using System.Drawing;

namespace MMI_SP
{
    class MenuMMI
    {
        // Constantes
        private const string NotifyChar = "CHAR_CARSITE";
        private const string NotifyTitle = "Mors Mutual";
        private const string NoMoneyMsg = "No tienes suficiente dinero.";

        private MenuPool _menuPool;
        private UIMenu _mainMenu;
        private UIMenuItem _itemInsure;

        internal void MenuPoolProcessMenus() { _menuPool.ProcessMenus(); }

        public MenuMMI()
        {
            _menuPool = new MenuPool();
            _mainMenu = new UIMenu("Mors Mutual", "Menú de seguros");
            _menuPool.Add(_mainMenu);
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
        }

        internal void Create()
        {
            // Banner
            if (System.IO.File.Exists(Config.BannerImage))
                _mainMenu.SetBannerType(Config.BannerImage);

            BuildItemInsure();
        }

        internal void Reset()
        {
            _mainMenu.Clear();
            Create();
        }

        /// <summary>
        /// Suscribe un callback al evento de cierre del menú principal.
        /// </summary>
        public Action OnMainMenuClosed(Action callback)
        {
            MenuCloseEvent handler = (sender) => callback();
            _mainMenu.OnMenuClose += handler;
            return () => _mainMenu.OnMenuClose -= handler;
        }

        // -------------------------------------------------------
        // BOTÓN "ASEGURAR VEHÍCULO"
        // -------------------------------------------------------
        private void BuildItemInsure()
        {
            if (InsuranceManager.Instance == null) return;

            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            // Evitar duplicados
            if (_itemInsure != null)
            {
                _itemInsure.Activated -= OnInsureActivated;
                _mainMenu.MenuItems.Remove(_itemInsure);
            }

            int cost = InsuranceManager.GetVehicleInsuranceCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = InsuranceManager.IsVehicleInsured(veh);
            bool isInsurable = InsuranceManager.IsVehicleInsurable(veh);

            string title = "Asegurar vehículo";
            string description;
            bool enabled = true;

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
            }
            else
            {
                description = $"Coste: {cost}$\n{vehName}";
            }

            _itemInsure = new UIMenuItem(title, description);
            _itemInsure.Enabled = enabled;
            if (enabled)
                _itemInsure.Activated += OnInsureActivated;

            _mainMenu.AddItem(_itemInsure);
        }

        private void OnInsureActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            Vehicle lastVeh = Game.Player.LastVehicle;
            if (lastVeh == null || !lastVeh.Exists()) return;

            if (InsuranceManager.Instance.IsVehicleInsured(lastVeh)) return;
            if (!InsuranceManager.IsVehicleInsurable(lastVeh)) return;

            int cost = InsuranceManager.GetVehicleInsuranceCost(lastVeh);

            // Comprobación de dinero (sin sonido, sin iFruit)
            if (Game.Player.Money < cost)
            {
                Utils.ShowNotification(NotifyChar, NotifyTitle, NoMoneyMsg, "");
                return;
            }

            Game.Player.Money -= cost;
            InsureVehicle(lastVeh);
        }

        private void InsureVehicle(Vehicle veh)
        {
            InsuranceManager.Instance.InsureVehicle(veh);
            Utils.ShowNotification(NotifyChar, NotifyTitle, "Vehículo asegurado correctamente.", "");
            _itemInsure.Enabled = false;
        }
    }
}