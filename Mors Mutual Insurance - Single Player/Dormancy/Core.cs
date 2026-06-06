using GTA;
using MMI_SP.DB;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Dormancy
{
    public sealed class DormantVehicle {
        internal VehicleData Data;
    }

    public static class Core
    {
        public static Result<bool> Despawn(string vehId, Vehicle veh) => DormancyService.Despawn(vehId, veh);
        public static Result<bool> MarkAsDormant(string vehId) => DormancyService.MarkAsDormant(vehId);
        public static Result<Vehicle> Respawn(string vehId) => DormancyService.Respawn(vehId);
        public static bool RemoveDormantByKey(string fullKey) => DormancyService.RemoveDormantByKey(fullKey);
        internal static IReadOnlyList<DormantVehicle> GetDormantList() => DormancyService.GetDormantList();

    }
}