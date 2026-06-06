using System.Collections.Generic;
using GTA;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Delivery
{
	public static class Manager
	{
		private static List<Data> _incomingVehicles = new List<Data>();

		public static bool HasActiveDelivery() => _incomingVehicles.Count > 0;

		public static void RequestDelivery(Vehicle veh, int cost, bool instant, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove)
		{
			Bring.Execute(veh, cost, instant, recoveredVehicle, blipsToRemove, _incomingVehicles);
		}

		internal static void UpdateIncomingVehicles() => TrackVehicleState.Execute(_incomingVehicles);

		internal static void CannotBringVehicle(Data incoming, int refund = Constants.ZERO) => Completion.Execute(incoming, refund,
			_incomingVehicles);

		internal static List<Vehicle> GetBringableVehicles(List<Vehicle> recovered, List<Vehicle> insured)
		{
			return Bring.GetBringableVehicles(recovered, insured);
		}

		public static void ForceArrivalIfNear() => Completion.ForceIfNear(_incomingVehicles);
	}
}