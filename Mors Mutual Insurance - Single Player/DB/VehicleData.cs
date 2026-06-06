using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMI_SP.DB
{
    public sealed class VehicleData
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        // Identidad
        [JsonProperty] public string Id { get; private set; }
        [JsonProperty] public string ModelName { get; private set; }
        [JsonProperty] public string Plate { get; private set; }
        [JsonProperty] public int PlateStyle { get; private set; }
        [JsonProperty] public string VehicleType { get; private set; }

        // Colores y carrocería
        [JsonProperty] public int PrimaryColor { get; private set; }
        [JsonProperty] public int SecondaryColor { get; private set; }
        [JsonProperty] public int WindowTint { get; private set; }

        // Ruedas y neumáticos
        [JsonProperty] public int WheelType { get; private set; }
        [JsonProperty] public int WheelColor { get; private set; }
        [JsonProperty] public int TireSmokeColor { get; private set; }
        [JsonProperty] public bool BulletproofTires { get; private set; }
        [JsonProperty] public bool CustomTires { get; private set; }

        // Neón
        [JsonProperty] public bool NeonLeft { get; private set; }
        [JsonProperty] public bool NeonRight { get; private set; }
        [JsonProperty] public bool NeonFront { get; private set; }
        [JsonProperty] public bool NeonBack { get; private set; }
        [JsonProperty] public int NeonColor { get; private set; }

        // Posición
        [JsonProperty] public float PosX { get; private set; }
        [JsonProperty] public float PosY { get; private set; }
        [JsonProperty] public float PosZ { get; private set; }
        [JsonProperty] public float Heading { get; private set; }

        // Modificaciones
        [JsonProperty] public Dictionary<int, int> Mods { get; private set; }

        // Estado
        [JsonProperty] public bool IsDestroyed { get; private set; }
        [JsonProperty] public DateTime DestroyedAt { get; private set; }
        [JsonProperty] public bool IsLocked { get; private set; }
        [JsonProperty] public bool IsDormant { get; private set; }
        [JsonProperty] public bool IsInGarage { get; private set; }

        public VehicleData(
            string id, string modelName, string plate,
            int primaryColor, int secondaryColor, bool isDestroyed,
            int windowTint = 0,
            int wheelType = 0,
            int wheelColor = 0,
            int tireSmokeColor = -1,
            bool neonLeft = false,
            bool neonRight = false,
            bool neonFront = false,
            bool neonBack = false,
            int neonColor = -1,
            bool bulletproofTires = false,
            float posX = 0f,
            float posY = 0f,
            float posZ = 0f,
            float heading = 0f,
            Dictionary<int, int> mods = null,
            DateTime destroyedAt = default,
            bool isLocked = false,
            int plateStyle = 0,
            bool customTires = false,
            bool isDormant = false,
            bool isInGarage = false,
            string vehicleType = "")
        {
            Id = id;
            ModelName = modelName;
            Plate = plate;
            PlateStyle = plateStyle;
            VehicleType = vehicleType;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            IsDestroyed = isDestroyed;
            WindowTint = windowTint;
            WheelType = wheelType;
            WheelColor = wheelColor;
            TireSmokeColor = tireSmokeColor;
            BulletproofTires = bulletproofTires;
            CustomTires = customTires;
            NeonLeft = neonLeft;
            NeonRight = neonRight;
            NeonFront = neonFront;
            NeonBack = neonBack;
            NeonColor = neonColor;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Heading = heading;
            Mods = mods ?? new Dictionary<int, int>();
            DestroyedAt = destroyedAt == default ? DateTime.MinValue : destroyedAt;
            IsLocked = isLocked;
            IsDormant = isDormant;
            IsInGarage = isInGarage;
        }
    }
}