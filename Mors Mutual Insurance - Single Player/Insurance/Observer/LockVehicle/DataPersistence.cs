using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class DataPersistence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Persist(Insurer insurer, string vehicleId, bool isLocked)
        {
            DB.Core.FindVehicle(vehicleId)
                .match<bool>(
                    onSome: existing =>
                    {
                        var updated = existing.With(d => d.IsLocked = isLocked);

                        DB.Core.Update(updated).match<bool>(
                            onOk: _ => true,
                            onErr: error =>
                            {
                                Logger.Error($"Error al persistir estado de bloqueo: {error}");
                                return false;
                            }
                        );
                        return true;
                    },
                    onNone: () =>
                    {
                        Logger.Error($"No se encontró VehicleData al persistir bloqueo para: {vehicleId}");
                        return false;
                    }
                );
        }
    }
}