using MMI_SP.Config;
using MMI_SP.Helpers;

namespace MMI_SP.iFruit.Config
{
    internal class ConfigMenu : MenuBase
    {
        public ConfigMenu() : base("Configuración", "Ajustes del mod", null) { }

        protected override void Build()
        {
            ConfigMenuBuilder.AddCheckbox(_mainMenu, "General", "PersistentInsuredVehicles",
                ModSettings.PersistentVehicles, "Vehículos persistentes");

            BuildiFruit();
            BuildInsurance();
            BuildBringVehicle();
            BuildVehicleUtilities();
        }

        private void BuildiFruit()
        {
            var sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "iFruit");
            ConfigMenuBuilder.AddListInt(sub, "iFruit", "PhoneVolume", ModSettings.iFruitVolume, "Volumen del teléfono",
                Constants.PHONE_VOLUME_MIN, Constants.PHONE_VOLUME_MAX, Constants.PHONE_VOLUME_STEP);
        }

        private void BuildInsurance()
        {
            var sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Seguros");
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "InsuranceCostMultiplier", ModSettings.InsuranceMult, "Multiplicador seguro",
                Constants.INSURANCE_MULT_MIN, Constants.INSURANCE_MULT_MAX, Constants.INSURANCE_MULT_STEP);
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "RecoverCostMultiplier", ModSettings.RecoverMult, "Multiplicador recuperación",
                Constants.INSURANCE_MULT_MIN, Constants.INSURANCE_MULT_MAX, Constants.INSURANCE_MULT_STEP);
        }

      private void BuildBringVehicle()
        {
            var sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Traer vehículo");
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleBasePrice", ModSettings.BringVehicleBasePrice, "Precio base",
                Constants.BRING_VEHICLE_PRICE_MIN, Constants.BRING_VEHICLE_PRICE_MAX, Constants.BRING_VEHICLE_PRICE_STEP);
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleRadius", ModSettings.BringVehicleRadius, "Radio de búsqueda",
                Constants.BRING_VEHICLE_RADIUS_MIN, Constants.BRING_VEHICLE_RADIUS_MAX, Constants.BRING_VEHICLE_RADIUS_STEP);
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleTimeout", ModSettings.BringVehicleTimeout, "Tiempo de espera",
                Constants.BRING_VEHICLE_TIMEOUT_MIN, Constants.BRING_VEHICLE_TIMEOUT_MAX, Constants.BRING_VEHICLE_TIMEOUT_STEP);
        }
        
      private void BuildVehicleUtilities()
      {
         var sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Utilidades de vehículos");
         ConfigMenuBuilder.AddActionItem(sub, "Buscar vehículos perdidos", () => Helpers.VehicleRecoveryHelper.RecoverLostVehicles());
         ConfigMenuBuilder.AddActionItem(sub, "Eliminar duplicados", () => Helpers.VehicleRecoveryHelper.CleanDuplicatesInRadius());
      }
    }
}