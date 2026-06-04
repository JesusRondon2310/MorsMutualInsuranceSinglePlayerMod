using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class MissingVehicles
    {
        internal static void Handle(List<Vehicle> insuredVehList, int i, Vehicle currentVeh)
        {
            string vehId = VehicleIdentifier.Get(currentVeh);

            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none())
            {
                insuredVehList.RemoveAt(i);
                return;
            }

            var data = dataOption.match<VehicleData>(
                onSome: vd => vd,
                onNone: () => null
            );

            if (data.IsDestroyed || data.IsDormant)
            {
                insuredVehList.RemoveAt(i);
                return;
            }

            if (data.IsLocked)
            {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            if (PoliceImpound.TryMarkAsImpounded(data, vehId, insuredVehList, i)) return;

            string recoveryKey = VehicleKey.FullKeyFrom(data);
            bool isRecoveredVehicle = Manager.RecoveredVehicleKeys.Contains(recoveryKey);

            Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            float distanceToPlayer = Game.Player.Character.Position.DistanceTo(lastPos);

            if (distanceToPlayer < 600f || (isRecoveredVehicle && !Game.Player.Character.IsInVehicle(currentVeh)))
            {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            Dormancy.Core.MarkAsDormant(vehId);
            insuredVehList.RemoveAt(i);
            Logger.Warning($"[VehicleMonitor] Despawn involuntario del engine: {vehId}");
        }
    }
}