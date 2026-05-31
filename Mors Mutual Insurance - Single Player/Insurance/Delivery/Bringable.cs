using System.Collections.Generic;
using GTA;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Bringable
    {
        internal static List<Vehicle> Execute(List<Vehicle> recoveredVehList, List<Vehicle> insuredVehList)
        {
            List<Vehicle> vehiclesToBring = new List<Vehicle>(recoveredVehList);
            foreach (Vehicle v in insuredVehList)
                if (!vehiclesToBring.Contains(v)) vehiclesToBring.Add(v);

            if (Game.Player.Character.CurrentVehicle != null &&
                vehiclesToBring.Contains(Game.Player.Character.CurrentVehicle))
                vehiclesToBring.Remove(Game.Player.Character.CurrentVehicle);

            return vehiclesToBring;
        }
    }
}