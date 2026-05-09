using System;
using System.Collections.Generic;
using System.IO;
using GTA;
using MMI_SP.Common;

namespace MMI_SP
{
    public class InsuranceManager
    {
        // Instancia única
        private static InsuranceManager _instance;
        public static InsuranceManager Instance => _instance;

        // Lista de vehículos asegurados (identificadores modelo_matrícula)
        private List<string> _insuredVehicles = new List<string>();

        // Ruta del archivo de persistencia
        private static string DatabasePath => Path.Combine(Config.BaseDir, "asegurados.txt");

        // Inicialización del sistema de seguros
        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new InsuranceManager();
                _instance.LoadDatabase();
                Logger.Debug("InsuranceManager inicializado.");
            }
        }

        // Constructor privado (patrón singleton)
        private InsuranceManager()
        {
            // La carga se hace en Initialize
        }

        // Carga la lista de vehículos desde el archivo TXT
        private void LoadDatabase()
        {
            if (File.Exists(DatabasePath))
            {
                _insuredVehicles = new List<string>(File.ReadAllLines(DatabasePath));
                Logger.Debug($"Se cargaron {_insuredVehicles.Count} vehículos asegurados.");
            }
            else
            {
                Logger.Debug("No se encontró base de datos de asegurados. Se comienza con lista vacía.");
            }
        }

        // Guarda la lista de vehículos en el archivo TXT
        private void SaveDatabase()
        {
            File.WriteAllLines(DatabasePath, _insuredVehicles);
            Logger.Debug("Base de datos de asegurados guardada.");
        }

        // Comprueba si un vehículo ya está asegurado
        public bool IsVehicleInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            string identifier = Utils.GetVehicleIdentifier(veh);
            return _insuredVehicles.Contains(identifier);
        }

        // Asegura un vehículo y guarda la lista
        public void InsureVehicle(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return;
            string identifier = Utils.GetVehicleIdentifier(veh);

            if (!_insuredVehicles.Contains(identifier))
            {
                _insuredVehicles.Add(identifier);
                SaveDatabase();
                Logger.Debug($"Vehículo asegurado: {identifier}");
            }
        }

        // Calcula el coste del seguro para un vehículo
        public int GetVehicleInsuranceCost(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return 0;

            // Coste base + extra por tipo de vehículo
            int baseCost = 500;
            if (veh.Model.IsHelicopter || veh.Model.IsPlane)
                baseCost += 5000;
            else if (veh.Model.IsBoat)
                baseCost += 2000;
            else if (veh.Model.IsBike || veh.Model.IsQuadBike)
                baseCost += 300;
            else if (veh.ClassType == VehicleClass.Military)
                baseCost += 10000;
            else
                baseCost += 1000; // coches, todoterrenos, etc.

            return (int)(baseCost * Config.InsuranceMult);
        }

        // Comprueba si un vehículo es asegurable
        public static bool IsVehicleInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   (veh.Model.IsCar || veh.Model.IsBike || veh.Model.IsQuadBike ||
                    veh.Model.IsHelicopter || veh.Model.IsPlane || veh.Model.IsBoat);
        }
    }
}