using System.Collections.Generic;
using GTA;
using MMI_SP.Insurance.Delivery.Incoming;

namespace MMI_SP.Insurance.Delivery
{
    public static class Manager
    {
        private static List<Data> _incomingVehicles = new List<Data>();

        public static bool HasActiveDelivery() => _incomingVehicles.Count > 0;

        public static void RequestDelivery(
            Vehicle veh, int cost, bool instant, bool recoveredVehicle,
            List<Vehicle> recoveredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            Bring.Execute(veh, cost, instant, recoveredVehicle, recoveredVehList, blipsToRemove, _incomingVehicles);
        }

        internal static void UpdateIncomingVehicles()
        {
            TrackVehicleState.Execute(_incomingVehicles);
        }

        internal static void CannotBringVehicle(Data incoming, int refund = 0)
        {
            Fail.Execute(incoming, refund, _incomingVehicles);
        }

        internal static List<Vehicle> GetBringableVehicles(List<Vehicle> recovered, List<Vehicle> insured)
        {
            return Bringable.Execute(recovered, insured);
        }

        public static void ForceArrivalIfNear()
        {
            Arrival.ForceIfNear(_incomingVehicles);
        }
    }
}