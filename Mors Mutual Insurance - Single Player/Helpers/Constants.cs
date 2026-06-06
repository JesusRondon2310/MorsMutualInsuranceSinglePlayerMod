using GTA;
using System.Drawing;

namespace MMI_SP.Helpers
{
   public static class Constants
   {
      // ==========================================
      // BLOQUE: Tiempos de espera y Valores numéricos genéricos
      // ==========================================
      public const int NONE = 0;           // Para comparaciones de cantidad (0 elementos)
      public const int FIRST_INDEX = 0;    // Para acceder al primer elemento de una lista
      public const int ZERO = 0;
      public const int ONE = 1;
      public const int CLOSEST_VEHICLE_NODE_INDEX = 1;   // Obtener el nodo más cercano
      public const int VEHICLE_NODE_TYPE = 1;            // Tipo nodo: vehículo
      public const int VEHICLE_NODE_FLAGS = 0;           // Sin flags especiales
      public const float DEFAULT_HEADING = 0f;           // Orientación por defecto
      public const float GROUND_Z_FALLBACK = 0f;         // Altura de suelo por defecto (fallback)
      public const int SHORT_TIMEOUT_MS = 1500;
      public const int MEDIUM_TIMEOUT_MS = 2000;
      public const int LONG_TIMEOUT_MS = 3000;

      // ==========================================
      // BLOQUE: Costes de Seguros
      // ==========================================
      public const int COST_BIKE = 1350;        // 1.350 GTA$
      public const int COST_TANK = 50000;       // 50.000 GTA$
      public const int COST_MILITARY = 17000;   // 17.000 GTA$
      public const int COST_CAR_DEFAULT = 5000; // 5.000 GTA$
      public const float RECOVER_PERCENT = 0.25f; // 25% para el deducible

      // ==========================================
      // BLOQUE: Límites del Mod
      // ==========================================
      public const int MAX_INSURED_VEHICLES = 30;
      public const int MAX_CLAIMABLE_VEHICLES = 5;

      // ==========================================
      // BLOQUE: Bloqueo de vehículos (LockVehicle)
      // ==========================================
      public const int LOCK_LIGHTS_ON_DURATION_MS = 200;
      public const int LOCK_LIGHTS_OFF_DURATION_MS = 200;
      public const int LOCK_HORN_DURATION_MS = 300;
      public const int LOCK_INITIAL_DELAY_MS = 100;
      public const int LOCK_LIGHTS_MODE_OFF = 0;
      public const int LOCK_LIGHTS_MODE_NORMAL = 1;
      public const int LOCK_LIGHTS_MODE_HAZARD = 2;
      public const float LOCK_MAX_DISTANCE = 2.5f;

      // ==========================================
      // BLOQUE: Dormancia
      // ==========================================
      public const float DORMANCY_THRESHOLD = 600f;
      public const float MIN_VALID_POSITION_LENGTH = 1f;

      // ==========================================
      // BLOQUE: Entregas / Spawn / Coordenadas / Teleport
      // ==========================================
      public const float MIN_DISTANCE_FOR_DELIVERY = 200f;
      public const float DELIVERY_DISTANCE_BEHIND_PLAYER = 150f;
      public const float MAX_ROAD_HEIGHT_DIFF = 3.0f;
      public const int CLOSEST_ROAD = 1; // carretera normal
      public const int GET_CLOSEST_VALID_ROAD = 1 | 2 | 4; // excluye trenes, contraflujo y autopistas 
      public const float VALID_ROAD_SEARCH_RADIUS = 30.0f; // radio de búsqueda 
      public const float TOO_CLOSE_DISTANCE = 40f;   // Distancia mínima para considerar vehículo demasiado cerca
      public const float CLOSEST_ROAD_SEARCH_RADIUS = 3.0f;
      public const float GROUND_OFFSET = 0.5f;
      public const float VEHICLE_STOP_SPEED = 0f;
      public const float VEHICLE_SPEED_THRESHOLD = 10.0f;          // Velocidad de conducción del mecánico (m/s)
      public const float ARRIVAL_PRECISION_DIST = 15.0f;   // distancia horizontal para considerar llegada
      public const float ARRIVAL_PRECISION_ALT = 2.0f;    // diferencia de altura para considerar llegada
      public const float ARRIVAL_DISTANCE = 35.0f;
      public const float ARRIVAL_MINIMUN_SPEED  = 3.0f;
      public const float ARRIVAL_RADIUS = 20.0f;         // Radio de seguimiento de ruta
      public const int TELEPORT_FREEZE_MS = 250;
      public const int MINUTE_MS = 60000;  // milisegundos en un minuto

      // ==========================================
      // BLOQUE: Modificaciones de vehículos (VehicleDataBuilder.cs)
      // ==========================================
      public const int MAX_VEHICLE_MOD_TYPE = 49;      // Tipos de mod (0..48)
      public const int TURBO_MOD_INDEX = 18;           // Índice del turbo en Mods
      public const int XENON_MOD_INDEX = 22;           // Índice de luces de xenón
      public const int VARIATION_MOD_10 = 10;          // Mod de variación 10
      public const int CUSTOM_TIRES_MOD_INDEX = 23;    // Índice de neumáticos personalizados
      public const int NO_MOD = -1;                    // Valor cuando una modificación no está instalada
      public const int NO_COLOR = -1;                  // Valor cuando no hay color (ej. neón apagado)
      public const int MOD_INSTALLED = 1;              // Valor cuando un mod toggle está activado
      public const float GARAGE_SEARCH_RADIUS = 30f;

      // ==========================================
      // BLOQUE: RecoverNodeSelector / MMIWarehouse
      // ==========================================
      public const float DISTANCE_IN_FRONT_OF_PLAYER = 5.0f;
      public const float VEHICLE_STOP_MIN_SPEED = 1.0f;
      public const float VEHICLE_LONG_THRESHOLD = 7.4f;

      // ==========================================
      // BLOQUE: Garajes y depósitos
      // ==========================================
      public const float INTERIOR_GARAGE_RADIUS = 20f;
      public const float LS_POLICE_IMPOUND_RADIUS = 11f;

      // ==========================================
      // BLOQUE: Claves de recuperación (KeyManager)
      // ==========================================
      public const int PLATE_INDEX = 1;          // índice de la placa en "Model_Plate_Id"
      public const int RECOVERY_KEY_PARTS = 3;   // número esperado de partes: Model, Plate, Id

      // ==========================================
      // BLOQUE: Placa problemática
      // ==========================================
      public const string PROBLEMATIC_PLATE = "46EEK572";

      // ==========================================
      // BLOQUE: Sonidos
      // ==========================================
      public const string SOUND_ARRIVE_TONE = "Text_Arrive_Tone";
      public const string SOUND_PHONE_SET = "Phone_SoundSet";

      // ==========================================
      // BLOQUE: Interfaz de usuario (UI)
      // ==========================================
      public const int INSURANCE_ICON_DISPLAY_MS = 4270;
      public const float SPRITE_INSURANCE_X = 1225f;
      public const float SPRITE_INSURANCE_Y = 600f;
      public static readonly Color INSURED_COLOR = Color.FromArgb(35, 199, 128);   // Verde
      public static readonly Color NOT_INSURED_COLOR = Color.FromArgb(190, 0, 50); // Rojo

      // ==========================================
      // BLOQUE: Configuración del menú y Notificaciones(iFruit)
      // ==========================================
      public const int NOTIFY_ICON_MESSAGE = 1; // Icono de mensaje
      public const int NOTIFY_ICON_MAIL = 2; // Icono de mail
      public const int NOTIFY_ICON_CONTACT_ADDED = 3; // Icono de contacto añadido
      public const int NOTIFY_ICON_NONE = 4; // sin icono
      public const int NOTIFY_NO_HIERARCHY = 0; // sin jerarquía de texto
      public const int PHONE_VOLUME_MIN = 0;
      public const int PHONE_VOLUME_MAX = 100;
      public const int PHONE_VOLUME_STEP = 5;
      public const float INSURANCE_MULT_MIN = 0f;
      public const float INSURANCE_MULT_MAX = 10f;
      public const float INSURANCE_MULT_STEP = 0.1f;
      public const int BRING_VEHICLE_PRICE_MIN = 0;
      public const int BRING_VEHICLE_PRICE_MAX = 2000;
      public const int BRING_VEHICLE_PRICE_STEP = 50;
      public const int BRING_VEHICLE_RADIUS_MIN = 10;
      public const int BRING_VEHICLE_RADIUS_MAX = 2000;
      public const int BRING_VEHICLE_RADIUS_STEP = 5;
      public const int BRING_VEHICLE_TIMEOUT_MIN = 1;
      public const int BRING_VEHICLE_TIMEOUT_MAX = 30;
      public const int BRING_VEHICLE_TIMEOUT_STEP = 1;
      public const float FLOAT_EPSILON = 0.001f;   // Tolerancia para comparación de floats
      public const int ROUND_DECIMALS = 1;         // Número de decimales para redondeo

      // Bromas del mecánico (iFruit)
      public static readonly string[] CURRENT_VEHICLE_JOKES = new string[]
      {
            "Te reto a presionar el boton. No hay huevos.",
            "Ese eres tú, payaso.",
            "No clono autos con la mente, bro. Bájate primero y luego le das al botón.",
            "Mira abajo, genio. Ya estás en el auto. Dale al botón todo lo que quieras, no hago magia.",
            "¿Quieres que traiga el coche en el que vas montado?",
            "¿Y qué estás manejando? Veo que tienes 160 de IQ.",
            "¿Te lele la cabeshita?",
            "Espérame ahí, ya voy llegando. ¿Crees que nací ayer o qué?"
      };

      public static readonly string[] TOO_CLOSE_JOKES = new string[]
      {
            "¿En serio? Lo tienes al lado. ¡Usa las piernas!",
            "¿Bro? Camina dos pasos, no seas tan perezoso.",
            "No seáis tan flojo y caminá, vago."
      };

      // ==========================================
      // BLOQUE: Agency/
      // ==========================================

      // Agency/Reception
      public const float RECEPTION_DISTANCE = 4.0f;
      public const int RECEPTION_DIALOGUE_TIMEOUT_MS = 5000;
      public const float AGENCY_POS_X = -825.7242f;
      public const float AGENCY_POS_Y = -261.2752f;
      public const float AGENCY_POS_Z = 37.0000f;
      public const int AGENCY_BLIP_COLOR = 6;

      // Agency/Office/Ambient
      public const int PED_COMPONENT_VARIATION_COMPONENT = 2;   // Componente del atuendo
      public const int PED_COMPONENT_VARIATION_DRAWABLE = 0;    // Drawable index
      public const int PED_COMPONENT_VARIATION_TEXTURE = 2;     // Texture index
      public const int PED_COMPONENT_VARIATION_PALETTE = 0;     // Palette index
      public const int MAX_PROP_CREATION_ATTEMPTS = 5;
      public const int MORNING_START_HOUR = 2;      // Hora de inicio de la mañana
      public const int NOON_START_HOUR = 12;        // Hora de inicio del mediodía
      public const int AFTERNOON_START_HOUR = 14;   // Hora de inicio de la tarde

      // Componentes del atuendo (SET_PED_COMPONENT_VARIATION)
      public const int PED_COMPONENT_FACE = 0;
      public const int PED_COMPONENT_HAIR = 2;
      public const int PED_COMPONENT_TORSO = 3;
      public const int PED_COMPONENT_LEGS = 4;
      public const int PED_COMPONENT_FEET = 6;
      // Drawables
      public const int PED_DRAWABLE_FACE_DEFAULT = 0;
      public const int PED_DRAWABLE_HAIR_STYLE = 1;
      public const int PED_DRAWABLE_TORSO_STYLE = 1;
      public const int PED_DRAWABLE_LEGS_DEFAULT = 0;
      public const int PED_DRAWABLE_FEET_DEFAULT = 0;
      // Textures
      public const int PED_TEXTURE_DEFAULT = 0;
      public const int PED_TEXTURE_LEGS_VARIATION = 1;
      // Palettes
      public const int PED_PALETTE_DEFAULT = 0;
      // Prop (SET_PED_PROP_INDEX)
      public const int PED_PROP_SUNGLASSES = 1;
      public const int PED_PROP_DRAWABLE_NONE = 0;
      public const int PED_PROP_TEXTURE_NONE = 0;
      public const int PED_PROP_ATTACH_NONE = 0;
      // Animación
      public const float NPC_ANIM_SPEED = 1.0f;
      public const int NPC_ANIM_DURATION_INFINITE = -1;

      // Agency/Office/Entry
      public const float ENTRY_CHECK_RADIUS = 3.0f;
      public const int ENTRY_LOADING_DELAY_MS = 2000;
      public const int ENTRY_FADE_DURATION_MS = 1000;   // Duración del fade in/out al entrar/salir

      // Agency/MainMenu/Buttons
      public const int MIN_PARTS_FOR_PLATE = 2;   // Número mínimo de partes al dividir un ID de vehículo 

      // ==========================================
      // BLOQUE: Debug/Debug.cs
      // ==========================================
      public const float DEBUG_TEXT_SIZE = 0.25f;
      public const float DEBUG_TEXT_START_X = 0.0f;
      public const float DEBUG_TEXT_START_Y = 0.0f;
      public const float DEBUG_TEXT_LINE_HEIGHT = 10.0f;   // Espaciado entre líneas

      // ==========================================
      // BLOQUE: Debug/Trailer
      // ==========================================
      // Posiciones y orientaciones de personajes
      public const float MICHAEL_POS_X = -787.1056f;
      public const float MICHAEL_POS_Y = 185.9241f;
      public const float MICHAEL_POS_Z = 72.83529f;
      public const float MICHAEL_HEADING = 58.53875f;

      public const float FRANKLIN_POS_X = -18.88375f;
      public const float FRANKLIN_POS_Y = -1451.604f;
      public const float FRANKLIN_POS_Z = 30.58212f;
      public const float FRANKLIN_HEADING = 223.4324f;
      public const float FRANKLIN_CAR_POS_X = -25.07652f;
      public const float FRANKLIN_CAR_POS_Y = -1450.024f;
      public const float FRANKLIN_CAR_POS_Z = 30.1692f;
      public const float FRANKLIN_CAR_HEADING = 183.715f;

      public const float TREVOR_POS_X = 1984.025f;
      public const float TREVOR_POS_Y = 3817.162f;
      public const float TREVOR_POS_Z = 32.28379f;
      public const float TREVOR_HEADING = 228.1426f;

      public const float FREEMODE_POS_X = -777.3974f;
      public const float FREEMODE_POS_Y = 282.0237f;
      public const float FREEMODE_POS_Z = 85.77721f;
      public const float FREEMODE_HEADING = 179.5031f;
      public const float FREEMODE_TREVOR_POS_X = -778.6523f;
      public const float FREEMODE_TREVOR_POS_Y = 282.0237f;
      public const float FREEMODE_TREVOR_POS_Z = 85.78682f;
      public const float FREEMODE_TREVOR_HEADING = 179.5031f;

      // Componentes de vestimenta (SET_PED_COMPONENT_VARIATION / SET_PED_PROP_INDEX)
      public const int MICHAEL_PROP_INDEX = 1;
      public const int MICHAEL_PROP_DRAWABLE = 5;
      public const int MICHAEL_PROP_TEXTURE = 0;
      public const int MICHAEL_PROP_ATTACH = 0;

      public const int FREEMODE_COMPONENT_ACCESSORIES = 8;
      public const int FREEMODE_ACCESSORIES_DRAWABLE = 3;
      public const int FREEMODE_COMPONENT_FACE = 0;
      public const int FREEMODE_FACE_DRAWABLE = 4;
      public const int FREEMODE_COMPONENT_HAIR = 2;
      public const int FREEMODE_HAIR_DRAWABLE = 2;
      public const int FREEMODE_HAIR_TEXTURE = 4;
      public const int FREEMODE_COMPONENT_TORSO = 3;
      public const int FREEMODE_TORSO_DRAWABLE = 1;
      public const int FREEMODE_COMPONENT_LEGS = 4;
      public const int FREEMODE_LEGS_DRAWABLE = 0;
      public const int FREEMODE_LEGS_TEXTURE = 1;
      public const int FREEMODE_COMPONENT_FEET = 6;
      public const int FREEMODE_FEET_DRAWABLE = 0;
      public const int FREEMODE_COMPONENT_TORSO2 = 11;
      public const int FREEMODE_TORSO2_DRAWABLE = 4;
      public const int FREEMODE_PROP_INDEX = 1;
      public const int FREEMODE_PROP_DRAWABLE = 3;
      public const int FREEMODE_PROP_TEXTURE = 4;
      public const int FREEMODE_PROP_ATTACH = 0;

      // Franklín: vehículo Buffalo2 y configuración
      public const VehicleHash FRANKLIN_CAR_MODEL = VehicleHash.Buffalo2;
      public const VehicleColor FRANKLIN_CAR_COLOR = VehicleColor.PureWhite;
      public const VehicleWindowTint FRANKLIN_CAR_TINT = VehicleWindowTint.Limo;
      public const float FRANKLIN_CAR_DIRT = 0.0f;

      // Speech y animación genérica
      public const string DEFAULT_CURSE_SPEECH = "GENERIC_CURSE_HIGH";
      public const string FREEMODE_CURSE_ANIM_DICT = "mp_celebration@idles@male";
      public const string FREEMODE_CURSE_ANIM_NAME = "celebration_idle_m_b";
   }
}