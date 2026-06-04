using GTA;
using MMI_SP.Debug;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Policies
{
    public static class Manager
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Insurer _insurer;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            if (_insurer != null) return new Ok<bool>(true);

            return DB.Core.Load().match<Result<bool>>(
                onOk: _ =>
                {
                    _insurer = new Insurer();
                    _insurer.LoadFrom(DB.Core.GetAll());
                    return new Ok<bool>(true);
                },
                onErr: (string error) =>
                {
                    Logger.Error($"Error al cargar DB: {error}");
                    return new Err<bool>(error);
                }
            );
        }

        public static bool IsInsured(Vehicle veh) => _insurer?.IsInsured(veh) ?? false;
        public static bool IsInsurable(Vehicle veh) => Insurer.IsInsurable(veh);
        public static int GetCost(Vehicle veh) => Calculator.GetCost(veh);
        public static List<string> GetInsuredList() => _insurer?.GetInsuredList() ?? new List<string>();
        public static List<string> GetDestroyedList() => _insurer?.GetDestroyedList() ?? new List<string>();

        public static Result<bool> Insure(in Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("No hay vehículo que asegurar.");
            if (IsInsured(veh)) return new Err<bool>("El vehículo ya está asegurado.");
            if (!IsInsurable(veh)) return new Err<bool>("Este vehículo no se puede asegurar.");

            int cost = GetCost(veh);
            if (Game.Player.Money < cost) return new Err<bool>("No tienes suficiente dinero.");

            Game.Player.Money -= cost;
            return _insurer != null ? _insurer.Insure(veh) : new Err<bool>("El gestor de seguros no está inicializado.");
        }

        public static Result<bool> Cancel(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            if (_insurer == null) return new Err<bool>("El gestor de seguros no está inicializado.");

            return _insurer.Cancel(vehicleId);
        }

        public static Result<bool> RecoverVehicle(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            if (_insurer == null) return new Err<bool>("El gestor de seguros no está inicializado.");

            return DestroyedVehicleRecover.RecoverVehicle(vehicleId, _insurer).match<Result<bool>>(
                onOk: _ => new Ok<bool>(true),
                onErr: error => new Err<bool>(error)
            );
        }
    }
}