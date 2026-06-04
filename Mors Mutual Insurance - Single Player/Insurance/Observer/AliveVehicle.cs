using GTA;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Observer
{
    internal static class AliveVehicle
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Handle(Policies.Insurer insurer, Vehicle currentVeh)
        {
            VehiclePersistence.SetPersistence(currentVeh, true);
        }
    }
}