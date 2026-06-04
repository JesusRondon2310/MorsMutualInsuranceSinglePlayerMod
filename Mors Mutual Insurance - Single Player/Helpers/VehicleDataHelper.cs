using System;
using System.Collections.Generic;
using MMI_SP.DB;

namespace MMI_SP.Helpers
{
    public static class VehicleDataHelper
    {
        public static VehicleData With(this VehicleData original, Action<VehicleDataUpdate> update)
        {
            var updater = new VehicleDataUpdate(original);
            update(updater);
            return new VehicleData(
                original.Id, original.ModelName, original.Plate,
                original.PrimaryColor, original.SecondaryColor, updater.IsDestroyed ?? original.IsDestroyed,
                windowTint: updater.WindowTint ?? original.WindowTint,
                wheelType: updater.WheelType ?? original.WheelType,
                wheelColor: updater.WheelColor ?? original.WheelColor,
                tireSmokeColor: updater.TireSmokeColor ?? original.TireSmokeColor,
                bulletproofTires: updater.BulletproofTires ?? original.BulletproofTires,
                neonLeft: updater.NeonLeft ?? original.NeonLeft,
                neonRight: updater.NeonRight ?? original.NeonRight,
                neonFront: updater.NeonFront ?? original.NeonFront,
                neonBack: updater.NeonBack ?? original.NeonBack,
                neonColor: updater.NeonColor ?? original.NeonColor,
                posX: updater.PosX ?? original.PosX,
                posY: updater.PosY ?? original.PosY,
                posZ: updater.PosZ ?? original.PosZ,
                heading: updater.Heading ?? original.Heading,
                mods: updater.Mods ?? original.Mods,
                destroyedAt: updater.DestroyedAt ?? original.DestroyedAt,
                isLocked: updater.IsLocked ?? original.IsLocked,
                plateStyle: updater.PlateStyle ?? original.PlateStyle,
                customTires: updater.CustomTires ?? original.CustomTires,
                isDormant: updater.IsDormant ?? original.IsDormant,
                isInGarage: updater.IsInGarage ?? original.IsInGarage,
                vehicleType: updater.VehicleType ?? original.VehicleType
            );
        }

        public class VehicleDataUpdate
        {
            public int? WindowTint { get; set; }
            public int? WheelType { get; set; }
            public int? WheelColor { get; set; }
            public int? TireSmokeColor { get; set; }
            public bool? BulletproofTires { get; set; }
            public bool? NeonLeft { get; set; }
            public bool? NeonRight { get; set; }
            public bool? NeonFront { get; set; }
            public bool? NeonBack { get; set; }
            public int? NeonColor { get; set; }
            public float? PosX { get; set; }
            public float? PosY { get; set; }
            public float? PosZ { get; set; }
            public float? Heading { get; set; }
            public Dictionary<int, int> Mods { get; set; }
            public DateTime? DestroyedAt { get; set; }
            public bool? IsLocked { get; set; }
            public int? PlateStyle { get; set; }
            public bool? CustomTires { get; set; }
            public bool? IsDormant { get; set; }
            public bool? IsInGarage { get; set; }
            public string VehicleType { get; set; }
            public bool? IsDestroyed { get; set; }
            public VehicleDataUpdate(VehicleData original) { }
        }
    }
}