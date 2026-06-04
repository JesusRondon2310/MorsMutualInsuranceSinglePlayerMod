using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using static MMI_SP.Agency.ItemsManager;

namespace MMI_SP.Agency.Office.Ambient
{
    internal static class Builder
    {
        // ==========================================
        // BLOQUE 1: Propiedades estáticas
        // ==========================================
        private static Weather _storedWeather;
        private static Camera _camera;
        private static Ped _npc;

        internal static Weather CurrentWeather => _storedWeather;
        internal static Camera CurrentCamera => _camera;
        internal static Ped CurrentNpc => _npc;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static Result<bool> Build(OfficeItemsCollection items)
        {
            // 1. Clima
            _storedWeather = World.Weather;
            World.Weather = Weather.Clouds;

            // 2. Cámara
#pragma warning disable CS0618
            _camera = World.CreateCamera(Config.CameraPos, Vector3.Zero, GameplayCamera.FieldOfView);
#pragma warning restore CS0618
            if (_camera == null) return new Err<bool>("No se pudo crear la cámara de la oficina.");

            // 3. NPC
            var npcResult = NpcHandler.Create();
            if (npcResult is Err<Ped> errNpc) return new Err<bool>(errNpc.Message);
            _npc = npcResult.unwrap_or(null);

            if (items.Type == CollectionType.Night)
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _npc, 2, 0, 2, 0);

            var aiResult = NpcHandler.SetAI(_npc);
            if (aiResult is Err<bool> errAi) return new Err<bool>(errAi.Message);

            // 4. Props
            foreach (OfficeItem item in items)
            {
                var propResult = item.Init();
                if (propResult is Ok<Prop> okProp)
                {
                    Prop prop = okProp.Value;
                    if (prop != null && prop.Exists() && _npc != null && _npc.Exists())
                        _npc.SetNoCollision(prop, true);
                }
            }

            // 5. Cámara apunta al NPC
            if (_npc != null && _npc.Exists())
                _camera.PointAt(_npc.Bones[Bone.IKHead]);
            else
                _camera.PointAt(Config.NpcPos);

#pragma warning disable CS0618
            World.RenderingCamera = _camera;
#pragma warning restore CS0618
            GTA.UI.Screen.FadeIn(1000);
            Screen.UIHandler(1000);

            return new Ok<bool>(true);
        }
    }
}