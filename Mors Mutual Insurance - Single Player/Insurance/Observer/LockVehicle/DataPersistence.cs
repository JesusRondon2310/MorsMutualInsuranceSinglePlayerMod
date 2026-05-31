using MMI_SP.Debug;
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
                        var updated = new DB.VehicleData(
                            existing.Id, existing.ModelName, existing.Plate,
                            existing.PrimaryColor, existing.SecondaryColor, existing.IsDestroyed,
                            windowTint: existing.WindowTint,
                            wheelType: existing.WheelType,
                            wheelColor: existing.WheelColor,
                            tireSmokeColor: existing.TireSmokeColor,
                            neonLeft: existing.NeonLeft,
                            neonRight: existing.NeonRight,
                            neonFront: existing.NeonFront,
                            neonBack: existing.NeonBack,
                            neonColor: existing.NeonColor,
                            bulletproofTires: existing.BulletproofTires,
                            posX: existing.PosX,
                            posY: existing.PosY,
                            posZ: existing.PosZ,
                            heading: existing.Heading,
                            mods: existing.Mods,
                            destroyedAt: existing.DestroyedAt,
                            isLocked: isLocked,
                            isDormant: existing.IsDormant,
                            isInGarage: existing.IsInGarage,
                            vehicleType: existing.VehicleType);

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