using GTA;
using GTA.Math;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Delivery
{
    // ==========================================
    // BLOQUE: Datos de entrega
    // ==========================================
    internal class Data
    {
        internal Vehicle vehicle;
        internal Ped driver;
        internal int price;
        internal Vector3 destination;
        internal int calledTime;
        internal bool recovered;
        internal EntityPosition originalPosition;
        internal int startTime;
        internal bool teleported;
    }

    // ==========================================
    // BLOQUE: Creación del conductor y la entrega
    // ==========================================
    internal static class Driver
    {
        internal static Result<Data> Create(Data incoming, bool driveToDestination)
        {
            incoming.vehicle.PlaceOnGround();
            incoming.driver = World.CreatePed(PedHash.Mechanic01, incoming.vehicle.Position, incoming.vehicle.Heading);

            if (incoming.driver == null || !incoming.driver.Exists()) return new Err<Data>("No se pudo crear el conductor.");

            if (driveToDestination)
            {
                incoming.startTime = Game.GameTime;
                incoming.vehicle.LockStatus = VehicleLockStatus.None;
                incoming.vehicle.IsEngineRunning = true;
                incoming.driver.SetIntoVehicle(incoming.vehicle, VehicleSeat.Driver);
                incoming.driver.Task.DriveTo(incoming.vehicle, incoming.destination, Constants.DRIVE_TO_SPEED, 
                    VehicleDrivingFlags.DrivingModeStopForVehicles, Constants.DRIVE_TO_RADIUS);
            }

            return new Ok<Data>(incoming);
        }
    }

    // ==========================================
    // BLOQUE: Punto de entrada público
    // ==========================================
    internal static class Incoming
    {
        internal static Result<Data> BringVehicle(Vehicle veh, int cost, bool isRecovered)
            => Create(veh, cost, isRecovered, true, false);

        internal static Result<Data> BringVehicleTeleported(Vehicle veh, int cost, bool isRecovered)
            => Create(veh, cost, isRecovered, true, true);

        private static Result<Data> Create(Vehicle veh, int cost, bool isRecovered, bool driveToDestination, bool teleported = false)
        {
            if (veh == null || !veh.Exists())
                return new Err<Data>("El vehículo no existe.");

            Data incoming = new Data
            {
                vehicle = veh,
                price = cost,
                recovered = isRecovered,
                originalPosition = new EntityPosition(veh.Position, veh.Heading),
                destination = Game.Player.Character.Position,
                calledTime = Game.GameTime,
                teleported = teleported
            };

            return Driver.Create(incoming, driveToDestination);
        }
    }
}