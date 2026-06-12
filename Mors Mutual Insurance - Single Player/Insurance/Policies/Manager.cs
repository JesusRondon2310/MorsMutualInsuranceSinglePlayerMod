using GTA;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Policies
{
    public static class Manager
    {
        // ==========================================
        // Inicialización (única excepción: llama a DB y crea Insurer)
        // ==========================================
        public static Result<bool> Initialize()
        {
            if (Insurer.Instance != null) return new Ok<bool>(true);

            return DB.Core.Load().match<Result<bool>>(
                onOk: _ => {
                    new Insurer(); // el constructor asigna Instance
                    return new Ok<bool>(true);
                },
                onErr: error => new Err<bool>(error)
            );
        }

        // ==========================================
        // Delegación a Calculator
        // ==========================================
        public static int GetCost(Vehicle veh) => Calculator.GetCost(veh);

        // ==========================================
        // Delegación a Insurer (estático)
        // ==========================================
        public static bool IsInsurable(Vehicle veh) => Insurer.IsInsurable(veh);

        // ==========================================
        // Delegación a Insurer.Instance
        // ==========================================
        public static bool IsInsured(Vehicle veh) => Insurer.Instance?.IsInsured(veh) ?? false;
        public static List<string> GetInsuredList() => Insurer.Instance?.GetInsuredList() ?? new List<string>();
        public static List<string> GetDestroyedList() => Insurer.Instance?.GetDestroyedList() ?? new List<string>();
        public static Result<bool> Insure(in Vehicle veh) => Insurer.Instance?.Insure(veh) ?? new Err<bool>("Seguro no inicializado.");
        public static Result<bool> Cancel(string vehicleId) => Insurer.Instance?.Cancel(vehicleId) ?? new Err<bool>("Seguro no inicializado.");
        public static Result<bool> MarkAsDestroyed(string vehicleId) => 
            Insurer.Instance?.MarkAsDestroyed(vehicleId) ?? new Err<bool>("Seguro no inicializado.");
        public static Result<bool> UpdateVehicleData(Vehicle veh) => 
            Insurer.Instance?.UpdateVehicleData(veh) ?? new Err<bool>("Seguro no inicializado.");
        public static List<DB.VehicleData> GetInsuredListFull() => Insurer.Instance?.GetInsuredListFull() ?? new List<DB.VehicleData>();

        // ==========================================
        // Delegación a DestroyedVehicleRecover
        // ==========================================
        public static Result<bool> RecoverVehicle(string vehicleId)
        {
            if (Insurer.Instance == null) return new Err<bool>("Seguro no inicializado.");
            return DestroyedVehicleRecover.RecoverVehicle(vehicleId, Insurer.Instance)
                .match<Result<bool>>(
                    onOk: _ => new Ok<bool>(true),
                    onErr: error => new Err<bool>(error)
                );
        }
    }
}