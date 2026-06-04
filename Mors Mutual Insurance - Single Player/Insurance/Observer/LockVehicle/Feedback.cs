using GTA;
using GTA.Native;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class Feedback
    {
        private const int LightsOnDuration = 200;
        private const int LightsOffDuration = 200;
        private const int HornDuration = 300;

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
            Step = 0;
            Running = true;
            NextStepTime = Game.GameTime + 100;
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
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, 2);
                    Function.Call(Hash.SET_HORN_PERMANENTLY_ON, SequenceVeh, HornDuration, 0, false);
                    NextStepTime = Game.GameTime + LightsOnDuration;
                    Step = 1;
                    break;
                case 1:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, 1);
                    NextStepTime = Game.GameTime + LightsOffDuration;
                    Step = 2;
                    break;
                case 2:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, 2);
                    Function.Call(Hash.SET_HORN_PERMANENTLY_ON, SequenceVeh, HornDuration, 0, false);
                    NextStepTime = Game.GameTime + LightsOnDuration;
                    Step = 3;
                    break;
                default:
                    Function.Call(Hash.SET_VEHICLE_LIGHTS, SequenceVeh, 0);
                    Running = false;
                    SequenceVeh = null;
                    Step = 0;
                    break;
            }
        }
    }
}