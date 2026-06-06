using GTA;
using MMI_SP.Config;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    internal static class Calculator
    {
        // ==========================================
        // BLOQUE: Coste base según tipo de vehículo
        // ==========================================

        private static int GetBaseCost(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return 0;

            if (veh.Model.IsBike || veh.Model.IsBicycle) return Constants.COST_BIKE;

            if (veh.Model == VehicleHash.Rhino || veh.Model == VehicleHash.Khanjali) return Constants.COST_TANK;

            if (veh.ClassType == VehicleClass.Military) return Constants.COST_MILITARY;

            return Constants.COST_CAR_DEFAULT;
        }

        private static int GetBaseCost(VehicleData data)
        {
            if (data == null)
                return 0;

            switch (data.VehicleType)
            {
                case "Bike": return Constants.COST_BIKE;
                
                case "Tank": return Constants.COST_TANK;
                
                case "Military": return Constants.COST_MILITARY;
                
                default: return Constants.COST_CAR_DEFAULT;
            }
        }

        // ==========================================
        // BLOQUE: API pública
        // ==========================================

        internal static int GetCost(Vehicle veh)
        {
            return (int)(GetBaseCost(veh) * ModSettings.InsuranceMult);
        }

        internal static int GetRecoverCost(string vehicleId)
        {
            return DB.Core.FindVehicle(vehicleId).match<int>(
                onSome: data =>
                {
                    int baseCost = GetBaseCost(data);
                    return (int)(baseCost * ModSettings.RecoverMult);
                },
                onNone: () => 0
            );
        }
    }
}