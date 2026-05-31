using GTA;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Blips
{
    public static class VehicleBlipHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<Blip> Create(Vehicle veh, bool flashing = false)
        {
            if (veh == null || !veh.Exists()) return new Err<Blip>("El vehículo no existe.");

            BlipSprite sprite = BlipSprite.PersonalVehicleCar;
            BlipColor color = BlipColor.Purple;

            if (veh.Model.IsBike || veh.Model.IsBicycle)
            {
                sprite = BlipSprite.PersonalVehicleBike;
                color = BlipColor.NetPlayer1;
            }
            else if (veh.Model == VehicleHash.Rhino || veh.Model == VehicleHash.Khanjali)
            {
                sprite = BlipSprite.Tank;
                color = BlipColor.Yellow;
            }
            else if (veh.ClassType == VehicleClass.Military)
            {
                sprite = BlipSprite.GunCar;
                color = BlipColor.Green;
            }

            Blip blip = veh.AddBlip();
            if (blip == null || !blip.Exists()) return new Err<Blip>("No se pudo crear el blip del vehículo.");

            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = "Vehículo asegurado";
            blip.IsShortRange = false;
            blip.IsFlashing = flashing;

            if (sprite == BlipSprite.Tank || sprite == BlipSprite.GunCar) blip.Rotation = (int)veh.Rotation.Z;

            return new Ok<Blip>(blip);
        }
    }
}