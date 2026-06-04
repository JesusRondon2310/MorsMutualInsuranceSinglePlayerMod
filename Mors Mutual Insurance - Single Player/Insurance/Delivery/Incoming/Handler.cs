using GTA;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Delivery.Incoming
{
    internal static class Handler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<Data> BringVehicle(Vehicle veh, int cost, bool isRecovered)
        {
            return Create(veh, cost, isRecovered, true);
        }

        private static Result<Data> Create(Vehicle veh, int cost, bool isRecovered, bool driveToDestination)
        {
            if (veh == null || !veh.Exists()) return new Err<Data>("El vehículo no existe.");

            Data incoming = new Data
            {
                vehicle = veh,
                price = cost,
                recovered = isRecovered,
                originalPosition = new EntityPosition(veh.Position, veh.Heading),
                destination = Game.Player.Character.Position,
                calledTime = Game.GameTime
            };

            return Driver.Create(incoming, driveToDestination);
        }
    }
}