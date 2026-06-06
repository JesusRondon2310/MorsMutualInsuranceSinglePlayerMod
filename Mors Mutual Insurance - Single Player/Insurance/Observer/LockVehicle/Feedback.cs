using GTA;
using GTA.Native;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class Feedback
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static bool Running { get; private set; } = false;
        internal static int Step { get; private set; } = 0;
        internal static int NextStepTime { get; private set; } = 0;
        internal static Vehicle SequenceVeh { get; private set; } = null;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Start(Vehicle veh)
        {
            SequenceVeh = veh;
            Step = Constants.FIRST_INDEX;
            Running = true;
            NextStepTime = Game.GameTime + Constants.LOCK_INITIAL_DELAY_MS;
        }

        internal static void Update()
        {
            if (!Running) return;
            if (Game.GameTime < NextStepTime) return;
            if (SequenceVeh == null || !SequenceVeh.Exists())
            {
                Running = false;
                return;
            }

            switch (Step)
            {
                case 0:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, Constants.LOCK_LIGHTS_MODE_HAZARD);
                    Function.Call(Hash.SET_HORN_PERMANENTLY_ON, SequenceVeh, Constants.LOCK_HORN_DURATION_MS, 0, false);
                    NextStepTime = Game.GameTime + Constants.LOCK_LIGHTS_ON_DURATION_MS;
                    Step = 1;
                    break;
                case 1:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, Constants.LOCK_LIGHTS_MODE_NORMAL);
                    NextStepTime = Game.GameTime + Constants.LOCK_LIGHTS_OFF_DURATION_MS;
                    Step = 2;
                    break;
                case 2:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, Constants.LOCK_LIGHTS_MODE_HAZARD);
                    Function.Call(Hash.SET_HORN_PERMANENTLY_ON, SequenceVeh, Constants.LOCK_HORN_DURATION_MS, 0, false);
                    NextStepTime = Game.GameTime + Constants.LOCK_LIGHTS_ON_DURATION_MS;
                    Step = 3;
                    break;
                default:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, Constants.LOCK_LIGHTS_MODE_OFF);
                    Running = false;
                    SequenceVeh = null;
                    Step = 0;
                    break;
            }
        }
    }
}