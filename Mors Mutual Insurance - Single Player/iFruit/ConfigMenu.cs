using MMI_SP.Config;
using NativeUI;
using GTA.Native;

namespace MMI_SP.iFruit
{
    internal class ConfigMenu
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private readonly MenuPool _pool;
        private readonly UIMenu _mainMenu;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal ConfigMenu()
        {
            _pool = new MenuPool();
            _mainMenu = new UIMenu("Configuración MMI", "Ajustes del mod");
            if (System.IO.File.Exists(ModSettings.BannerImage))
                _mainMenu.SetBannerType(ModSettings.BannerImage);
            _pool.Add(_mainMenu);

            // Bug #1: casilla de persistencia directa en el menú principal
            ConfigMenuBuilder.AddCheckbox(_mainMenu, "General", "PersistentInsuredVehicles",
                ModSettings.PersistentVehicles, "Vehículos persistentes");

            BuildiFruit();
            BuildInsurance();
            BuildBringVehicle();
            _pool.RefreshIndex();
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        internal void MenuPoolProcessMenus() => _pool.ProcessMenus();

        private void BuildiFruit()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "iFruit");
            ConfigMenuBuilder.AddListInt(sub, "iFruit", "PhoneVolume",
                ModSettings.iFruitVolume, "Volumen del teléfono", 0, 100, 5);
        }

        private void BuildInsurance()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Seguros");
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "InsuranceCostMultiplier",
                ModSettings.InsuranceMult, "Multiplicador seguro", 0f, 10f, 0.1f);
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "RecoverCostMultiplier",
                ModSettings.RecoverMult, "Multiplicador recuperación", 0f, 10f, 0.1f);
        }

        private void BuildBringVehicle()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Traer vehículo");
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleBasePrice",
                ModSettings.BringVehicleBasePrice, "Precio base", 0, 2000, 50);
            // Bug #2: eliminado checkbox "Entrega instantánea"
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleRadius",
                ModSettings.BringVehicleRadius, "Radio de búsqueda", 10, 2000, 5);
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleTimeout",
                ModSettings.BringVehicleTimeout, "Tiempo de espera", 1, 30, 1);
        }
    }
}