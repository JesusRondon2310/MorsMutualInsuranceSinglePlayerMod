using GTA;
using System;
using System.Collections.Generic;

namespace MMI_SP.Helpers
{
    public static class Timers
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly Dictionary<string, int> _timers = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> _intervals = new Dictionary<string, int>();
        private static readonly Dictionary<string, Func<int>> _randomIntervalProviders = new Dictionary<string, Func<int>>();

        // Timers dinámicos: ahora almacenan (startTime, durationMs, callback opcional)
        private static readonly Dictionary<string, (int startTime, int durationMs, Action callback)> _customTimers =
            new Dictionary<string, (int startTime, int durationMs, Action callback)>();

        // ==========================================
        // BLOQUE 2: Timers fijos
        // ==========================================
        public static void Register(string name, int intervalMs)
        {
            _intervals[name] = intervalMs;
            _timers[name] = 0;
        }

        public static bool CheckAndExecute(string name, Action callback)
        {
            if (!_timers.ContainsKey(name)) return false;
            if (Game.GameTime >= _timers[name])
            {
                callback();
                _timers[name] = Game.GameTime + _intervals[name];
                return true;
            }
            return false;
        }

        public static void Reset(string name)
        {
            if (_timers.ContainsKey(name))
                _timers[name] = 0;
        }

        public static void ResetAll()
        {
            var keys = new List<string>(_timers.Keys);
            foreach (var key in keys)
                _timers[key] = 0;
        }

        // ==========================================
        // BLOQUE 2b: Timers aleatorios
        // ==========================================
        public static void RegisterRandom(string name, Func<int> intervalProvider)
        {
            _randomIntervalProviders[name] = intervalProvider;
            _timers[name] = 0;
        }

        public static bool CheckAndExecuteRandom(string name, Action callback)
        {
            if (!_timers.ContainsKey(name) || !_randomIntervalProviders.ContainsKey(name)) return false;
            if (Game.GameTime >= _timers[name])
            {
                callback();
                int nextInterval = _randomIntervalProviders[name]();
                _timers[name] = Game.GameTime + nextInterval;
                return true;
            }
            return false;
        }

        // ==========================================
        // BLOQUE 3: Timers dinámicos
        // ==========================================
        public static void StartCustomTimer(string key, int durationMs, Action onExpired = null)
        {
            _customTimers[key] = (Game.GameTime, durationMs, onExpired);
        }

        public static bool IsCustomTimerExpired(string key)
        {
            if (!_customTimers.TryGetValue(key, out var timer)) return false;
            return Game.GameTime - timer.startTime >= timer.durationMs;
        }

        public static bool ExecuteWhenExpired(string key)
        {
            if (!_customTimers.TryGetValue(key, out var timer)) return false;
            if (Game.GameTime - timer.startTime < timer.durationMs) return false;

            timer.callback?.Invoke();
            _customTimers.Remove(key);
            return true;
        }

        public static void RemoveCustomTimer(string key)
        {
            _customTimers.Remove(key);
        }

        // ==========================================
        // BLOQUE 4: Registro de timers predeterminados
        // ==========================================
        internal static void Initialize()
        {
            /////////////// Observer/Manager.cs ///////////////
            Register("Insurance", 1000);
            Register("DetectInsuredVehicles", 3000);
            Register("RecoveredVehicle", 3000);
            Register("IncomingVehicle", 1000);
            Register("CleanupRecoveredKeys", 10000);
            /////////////// Observer/Manager.cs ///////////////

            /////////////// Delivery/TrackVehicleState.cs ///////////////
            // (usa timers dinámicos StartCustomTimer/IsCustomTimerExpired)
            /////////////// Delivery/TrackVehicleState.cs ///////////////

            /////////////// Agency/Office/Manager.cs ///////////////
            RegisterRandom("RandomSpeech", () => new Random(Game.GameTime).Next(10000, 20000));
            /////////////// Agency/Office/Manager.cs ///////////////

            /////////////// Observer/AliveVehicle.cs ///////////////
            Register("AliveVehicleSave", 10000);
            /////////////// Observer/AliveVehicle.cs ///////////////
        }
    }
}