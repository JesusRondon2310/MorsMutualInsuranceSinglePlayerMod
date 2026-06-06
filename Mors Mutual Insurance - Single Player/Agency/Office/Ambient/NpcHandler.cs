using GTA;
using GTA.Native;
using MMI_SP.Helpers;
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
                "base",
                Constants.NPC_ANIM_SPEED,
                Constants.NPC_ANIM_DURATION_INFINITE,
                AnimationFlags.Loop);
            npc.IsPositionFrozen = true;
            npc.Position = Config.NpcPos;
            npc.Rotation = Config.NpcRot;
            npc.Task.LookAt(Config.CameraPos);

            return new Ok<bool>(true);
        }

        private static void SetComponentVariations(Ped npc)
        {
            // Cara
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc,
                Constants.PED_COMPONENT_FACE,
                Constants.PED_DRAWABLE_FACE_DEFAULT,
                Constants.PED_TEXTURE_DEFAULT,
                Constants.PED_PALETTE_DEFAULT);
            // Pelo
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc,
                Constants.PED_COMPONENT_HAIR,
                Constants.PED_DRAWABLE_HAIR_STYLE,
                Constants.PED_TEXTURE_DEFAULT,
                Constants.PED_PALETTE_DEFAULT);
            // Torso
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc,
                Constants.PED_COMPONENT_TORSO,
                Constants.PED_DRAWABLE_TORSO_STYLE,
                Constants.PED_TEXTURE_DEFAULT,
                Constants.PED_PALETTE_DEFAULT);
            // Piernas
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc,
                Constants.PED_COMPONENT_LEGS,
                Constants.PED_DRAWABLE_LEGS_DEFAULT,
                Constants.PED_TEXTURE_LEGS_VARIATION,
                Constants.PED_PALETTE_DEFAULT);
            // Pies
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, npc,
                Constants.PED_COMPONENT_FEET,
                Constants.PED_DRAWABLE_FEET_DEFAULT,
                Constants.PED_TEXTURE_DEFAULT,
                Constants.PED_PALETTE_DEFAULT);
            // Gafas
            Function.Call(Hash.SET_PED_PROP_INDEX, npc,
                Constants.PED_PROP_SUNGLASSES,
                Constants.PED_PROP_DRAWABLE_NONE,
                Constants.PED_PROP_TEXTURE_NONE,
                Constants.PED_PROP_ATTACH_NONE);
        }
    }
}