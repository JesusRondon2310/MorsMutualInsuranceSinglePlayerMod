using GTA;
using GTA.Math;
using MMI_SP.Helpers.Spawn.Coordinates;

namespace MMI_SP.Insurance.Delivery.Incoming
{
    internal class Data
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        internal Vehicle vehicle;
        internal Ped driver;
        internal int price;
        internal Vector3 destination;
        internal int calledTime;
        internal bool recovered;
        internal EntityPosition originalPosition;
    }
}