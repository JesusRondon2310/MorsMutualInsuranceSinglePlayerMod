using System;
using System.Collections.Generic;
using GTA;

namespace MMI_SP.Insurance.Observer
{
    public class Manager : Script
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Manager _instance;
        internal static Manager Instance => _instance;
        internal static bool Initialized { get; private set; }

        private static List<Vehicle> _insuredVehList = new List<Vehicle>();
        private static List<Vehicle> _recoveredVehList = new List<Vehicle>();
        private static Dictionary<string, Blip> _blipsToRemove = new Dictionary<string, Blip>();

        private int _timerInsurance = 0;
        private int _timerDetectInsuredVehicles = 0;
        private int _timerRecoveredVehicle = 0;

        // ==========================================
        // BLOQUE 2: Constructor y eventos
        // ==========================================
        public Manager()
        {
            _instance = this;
            Tick += Initialize;
        }

        private void Initialize(object sender, EventArgs e)
        {
            while (!MMI.IsInitialized) Yield();

            Initialized = true;
            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Insurer.Instance == null) return;

            ProcessTimers();
            VehicleChangeHandler.Handle(_insuredVehList, _recoveredVehList, _blipsToRemove);
        }

        private void ProcessTimers()
        {
            if (_timerInsurance <= Game.GameTime)
            {
                VehicleMonitor.UpdateInsurance(Insurer.Instance, _insuredVehList, _blipsToRemove);
                _timerInsurance = Game.GameTime + 1000;
            }
            if (_timerRecoveredVehicle <= Game.GameTime)
            {
                RecoveryManager.UpdateRecoveredVehicles(_recoveredVehList);
                _timerRecoveredVehicle = Game.GameTime + 3000;
            }
            if (_timerDetectInsuredVehicles <= Game.GameTime)
            {
                VehicleMonitor.CheckForInsuredVehicles(Insurer.Instance, _insuredVehList, _blipsToRemove);
                _timerDetectInsuredVehicles = Game.GameTime + 3000;
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            BlipManager.ClearAllBlips(_blipsToRemove);
            PersistenceManager.RemovePersistence(_recoveredVehList);
        }
    }
}
