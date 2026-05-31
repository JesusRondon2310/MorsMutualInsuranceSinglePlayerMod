using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class NpcHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================

        internal static Ped CurrentNpc { get; private set; }

        public static Result<Ped> Create()
        {
            Ped npc = World.CreatePed(Config.NpcModel, Config.NpcPos);
            if (npc == null || !npc.Exists()) return new Err<Ped>("No se pudo crear el NPC de la oficina.");

            npc.IsPersistent = true;
            SetComponentVariations(npc);
            CurrentNpc = npc;
            return new Ok<Ped>(npc);
        }

        public static Result<bool> SetAI(Ped npc)
        {
            if (npc == null || !npc.Exists()) return new Err<bool>("El NPC no existe.");

            npc.Task.PlayAnimation(
                "amb@prop_human_seat_chair@female@arms_folded@base",
                "base", 1.0f, -1, AnimationFlags.Loop);
            npc.IsPositionFrozen = true;
            npc.Position = Config.NpcPos;
            npc.Rotation = Config.NpcRot;
            npc.Task.LookAt(Config.CameraPos);

            return new Ok<bool>(true);
        }

        private static void SetComponentVariations(Ped npc)
        {
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 0, 0, 0, 0); // Cara
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 2, 1, 0, 0); // Pelo
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 3, 1, 0, 0); // Torso
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 4, 0, 1, 0); // Piernas
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc, 6, 0, 0, 0); // Pies
            Function.Call(Hash.SET_PED_PROP_INDEX, npc, 1, 0, 0, 0);          // Gafas
        }
    }
}