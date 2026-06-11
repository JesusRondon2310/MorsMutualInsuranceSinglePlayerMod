using GTA;
using MMI_SP.DB;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Dormancy
{
    public sealed class DormantVehicle {
        internal VehicleData Data;
    }

    public static class Core
    {
        public static Result<bool> Despawn(string vehId, Vehicle veh) => DormancyLifeCycle.Despawn(vehId, veh);
        public static Result<bool> MarkAsDormant(string vehId) => DormancyLifeCycle.MarkAsDormant(vehId);
        public static Result<Vehicle> Respawn(string vehId) => DormancyLifeCycle.Respawn(vehId);
        public static bool RemoveDormantByKey(string fullKey) => DormancyLifeCycle.RemoveDormantByKey(fullKey);
        internal static IReadOnlyList<DormantVehicle> GetDormantList() => DormancyLifeCycle.GetDormantList();

    }
}