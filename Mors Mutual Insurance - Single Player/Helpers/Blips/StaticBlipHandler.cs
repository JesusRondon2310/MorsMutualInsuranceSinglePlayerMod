using GTA;
using GTA.Math;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Blips
{
    public static class StaticBlipHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<Blip> Create(Vector3 position, string name, BlipSprite sprite = BlipSprite.PersonalVehicleCar, 
            BlipColor color = BlipColor.White)
        {
            Blip blip = World.CreateBlip(position);
            if (blip == null || !blip.Exists()) return new Err<Blip>("No se pudo crear el blip estático.");

            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = string.IsNullOrEmpty(name) ? "Vehículo asegurado" : name;
            blip.IsShortRange = false;

            return new Ok<Blip>(blip);
        }

        public static void Remove(Blip blip)
        {
            if (blip != null && blip.Exists())
                blip.Delete();
        }
    }
}