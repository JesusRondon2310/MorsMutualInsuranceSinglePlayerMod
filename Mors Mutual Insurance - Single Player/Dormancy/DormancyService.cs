using System.Collections.Generic;
using System.Linq;
using GTA;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Dormancy
{
    internal static class DormancyService
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static List<DormantVehicle> DormantVehicles = new List<DormantVehicle>();

        // ==========================================
        // BLOQUE 2: Funciones auxiliares
        // ==========================================
        private static bool IsAlreadyDormant(VehicleData data) {
            return DormantVehicles.Any(d => d.Data?.Plate == data.Plate && d.Data?.ModelName == data.ModelName);
        }

        // ==========================================
        // BLOQUE 3: Funciones
        // ==========================================

        // Despawn (delega en DespawnHandler)
        internal static Result<bool> Despawn(string vehId, Vehicle veh)
        {
            if (string.IsNullOrEmpty(vehId)) return new Err<bool>("ID de vehículo no válido.");

            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");

            return DespawnHandler.Execute(vehId, veh, DormantVehicles);
        }

        // Marcar como dormante (con lógica propia)
        internal static Result<bool> MarkAsDormant(string vehId)
        {
            var dataOption = DB.Core.FindVehicle(vehId);
            return dataOption.match<Result<bool>>(
                onSome: data => {
                    if (IsAlreadyDormant(data)) return new Err<bool>("El vehículo ya está en estado dormante.");

                    var dormantData = data.With(d => { d.IsDormant = true; d.IsInGarage = false; });
                    DB.Core.Update(dormantData);

                    var dormant = new DormantVehicle { Data = dormantData };
                    DormantVehicles.Add(dormant);
                    return new Ok<bool>(true);
                },
                onNone: () => new Err<bool>("Vehículo no encontrado.")
            );
        }

        // Respawn (delega en RespawnHandler)
        internal static Result<Vehicle> Respawn(string vehId)
        {
            if (string.IsNullOrEmpty(vehId)) return new Err<Vehicle>("ID de vehículo no válido.");

            return RespawnHandler.Execute(vehId, DormantVehicles);
        }

        // Obtener lista de solo lectura
        internal static IReadOnlyList<DormantVehicle> GetDormantList() => DormantVehicles.AsReadOnly();

        internal static bool RemoveDormantByKey(string fullKey)
        {
            var dormant = DormantVehicles.FirstOrDefault(d => d.Data != null && VehicleKey.FullKeyFrom(d.Data) == fullKey);
            if (dormant != null) {
                DormantVehicles.Remove(dormant);
                return true;
            }
            return false;
        }
    }
}