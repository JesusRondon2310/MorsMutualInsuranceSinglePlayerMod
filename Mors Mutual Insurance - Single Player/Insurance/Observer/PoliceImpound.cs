using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class PoliceImpound
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static bool TryMarkAsImpounded(VehicleData data, string vehId, List<Vehicle> insuredVehList, int index)
        {
            Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            if (!VehiclesInGarage.IsPositionInPoliceImpound(lastPos))
                return false;

            var updatedData = data.With(d => { d.IsInGarage = true; d.IsDormant = false; });

            DB.Core.Update(updatedData);
            insuredVehList.RemoveAt(index);
            return true;
        }
    }
}