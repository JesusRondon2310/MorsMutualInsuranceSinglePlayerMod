using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using MMI_SP.Dialogue;
using MMI_SP.Agency.Office.Ambient;
using NativeUI;
using MMI_SP.Agency.MainMenu.Buttons;
using MMI_SP.Debug;
using GTA;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal class RecoverHandler
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        public static RecoverHandler Instance { get; private set; }

        private UIMenu _submenu;
        private readonly UIMenu _parentMenu;
        private readonly MenuPool _pool;
        public MenuPool Pool => _pool;
        public UIMenu Submenu => _submenu;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public RecoverHandler(UIMenu parentMenu, MenuPool pool)
        {
            Instance = this;
            _parentMenu = parentMenu;
            _pool = pool;
        }

        public void Build()
        {
            var submenuResult = Buttons.Build.SubMenu(
                _parentMenu,
                _pool,
                "Reclamar vehículo destruido",
                "Vehículos asegurados que han sido destruidos",
                "Selecciona el vehículo a recuperar",
                OnRecoverActivated,
                "No tienes vehículos destruidos.",
                showDestroyed: true);

            if (submenuResult is Ok<UIMenu> ok)
                _submenu = ok.Value;
            else
                Logger.Error($"Error al crear submenú Recuperar: {((Err<UIMenu>)submenuResult).Message}");
        }

        private void OnRecoverActivated(string vehicleId)
        {
            var result = Manager.RecoverVehicle(vehicleId);

            result.match<bool>(
                onOk: _ =>
                {
                    string vehName = GetLocalizedVehicleName(vehicleId);
                    Notification.ShowMMI("Información", $"Reclamación aprobada. Tu ~b~{vehName}~w~ está esperando por ti en el depósito.");
                    Core.PlayRandom(Core.SpeechType.OfficeSomething, NpcHandler.CurrentNpc);
                    Repopulate();
                    _submenu.Visible = true;
                    _parentMenu.Visible = false;
                    return true;
                },
                onErr: error =>
                {
                    Notification.ShowMMI("~r~ERROR~w~", error);
                    return false;
                }
            );
        }

        public void Repopulate()
        {
            ExecuteRebuild.SubMenu(() =>
            {
                var fillResult = Fill.SubMenu(_submenu, OnRecoverActivated, "No tienes vehículos destruidos.", showDestroyed: true);
                if (fillResult is Err<bool> err)
                    Logger.Error($"Error al repoblar submenú Recuperar: {err.Message}");
            }, _pool);
        }

        // Obtiene el nombre localizado del vehículo desde su ID en la base de datos.
        private static string GetLocalizedVehicleName(string vehicleId)
        {
            return DB.Core.FindVehicle(vehicleId).match<string>(
                onSome: data =>
                {
                    string name = Game.GetLocalizedString(data.ModelName);
                    return string.IsNullOrEmpty(name) ? data.ModelName : name;
                },
                onNone: () => "Desconocido"
            );
        }
    }
}