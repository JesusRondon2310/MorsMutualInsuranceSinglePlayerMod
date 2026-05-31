using GTA;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Delivery.Incoming
{
    internal static class Driver
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<Data> Create(Data incoming, bool driveToDestination)
        {
            incoming.vehicle.PlaceOnGround();
            incoming.driver = World.CreatePed(PedHash.Mechanic01, incoming.vehicle.Position, incoming.vehicle.Heading);

            if (incoming.driver == null || !incoming.driver.Exists())
                return new Err<Data>("No se pudo crear el conductor.");

            if (driveToDestination)
            {
                incoming.vehicle.LockStatus = VehicleLockStatus.None;
                incoming.vehicle.IsEngineRunning = true;
                incoming.driver.SetIntoVehicle(incoming.vehicle, VehicleSeat.Driver);
                incoming.driver.Task.DriveTo(incoming.vehicle, incoming.destination, 10.0f, VehicleDrivingFlags.DrivingModeStopForVehicles, 20.0f);
            }

            return new Ok<Data>(incoming);
        }
    }
}