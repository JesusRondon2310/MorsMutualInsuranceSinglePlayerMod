using GTA;
using GTA.Native;
using GTA.Math;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Debug
{
    internal static class Trailer
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        static Ped _michael, _franklin, _trevor, _freemode;

        // Michael
        readonly static Vector3 _michaelPosition = new Vector3(Constants.MICHAEL_POS_X, Constants.MICHAEL_POS_Y, Constants.MICHAEL_POS_Z);
        readonly static float _michaelHeading = Constants.MICHAEL_HEADING;

        // Franklin
        readonly static Vector3 _franklinPosition = new Vector3(Constants.FRANKLIN_POS_X, Constants.FRANKLIN_POS_Y, Constants.FRANKLIN_POS_Z);
        readonly static float _franklinHeading = Constants.FRANKLIN_HEADING;
        readonly static Vector3 _franklinCarPosition = new Vector3(Constants.FRANKLIN_CAR_POS_X, Constants.FRANKLIN_CAR_POS_Y, Constants.FRANKLIN_CAR_POS_Z);
        readonly static float _franklinCarHeading = Constants.FRANKLIN_CAR_HEADING;

        // Trevor
        readonly static Vector3 _trevorPosition = new Vector3(Constants.TREVOR_POS_X, Constants.TREVOR_POS_Y, Constants.TREVOR_POS_Z);
        readonly static float _trevorHeading = Constants.TREVOR_HEADING;

        // Freemode
        readonly static Vector3 _freemodePosition = new Vector3(Constants.FREEMODE_POS_X, Constants.FREEMODE_POS_Y, Constants.FREEMODE_POS_Z);
        readonly static float _freemodeHeading = Constants.FREEMODE_HEADING;
        readonly static Vector3 _freemodeTrevorPosition = new Vector3(Constants.FREEMODE_TREVOR_POS_X, Constants.FREEMODE_TREVOR_POS_Y, Constants.FREEMODE_TREVOR_POS_Z);
        readonly static float _freemodeTrevorHeading = Constants.FREEMODE_TREVOR_HEADING;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void CleanUp()
        {
            if (_michael != null) _michael.Delete();
            if (_franklin != null) _franklin.Delete();
            if (_trevor != null) _trevor.Delete();
            if (_freemode != null) _freemode.Delete();
        }

        internal static Result<bool> Spawn(PedHash hash)
        {
            switch (hash)
            {
                case PedHash.Michael:
                    _michael = World.CreatePed(PedHash.Michael, _michaelPosition, _michaelHeading);
                    if (_michael == null) return new Err<bool>("No se pudo crear a Michael.");
                    SetupMichael();
                    break;
                case PedHash.Franklin:
                    if (_franklin != null) _franklin.Delete();
                    _franklin = World.CreatePed(PedHash.Franklin, _franklinPosition, _franklinHeading);
                    if (_franklin == null) return new Err<bool>("No se pudo crear a Franklin.");
                    SetupFranklin();
                    break;
                case PedHash.Trevor:
                    if (_trevor != null) _trevor.Delete();
                    _trevor = World.CreatePed(PedHash.Trevor, _trevorPosition, _trevorHeading);
                    if (_trevor == null) return new Err<bool>("No se pudo crear a Trevor.");
                    break;
                case PedHash.FreemodeMale01:
                    if (_freemode != null) _freemode.Delete();
                    _freemode = World.CreatePed(PedHash.FreemodeMale01, _freemodePosition, _freemodeHeading);
                    if (_freemode == null) return new Err<bool>("No se pudo crear al Freemode.");
                    if (_trevor != null) _trevor.Delete();
                    _trevor = World.CreatePed(PedHash.Trevor, _freemodeTrevorPosition, _freemodeTrevorHeading);
                    if (_trevor == null) return new Err<bool>("No se pudo crear a Trevor (Freemode).");
                    SetFreemodeClothes();
                    break;
            }
            return new Ok<bool>(true);
        }

        private static void SetupMichael()
        {
            Function.Call(Hash.SET_PED_PROP_INDEX, _michael,
                Constants.MICHAEL_PROP_INDEX,
                Constants.MICHAEL_PROP_DRAWABLE,
                Constants.MICHAEL_PROP_TEXTURE,
                Constants.MICHAEL_PROP_ATTACH);
        }

        private static void SetupFranklin()
        {
            Vehicle buffalo = World.CreateVehicle(Constants.FRANKLIN_CAR_MODEL, _franklinCarPosition, _franklinCarHeading);
            buffalo.Mods.PrimaryColor = Constants.FRANKLIN_CAR_COLOR;
            buffalo.Mods.WindowTint = Constants.FRANKLIN_CAR_TINT;
            buffalo.DirtLevel = Constants.FRANKLIN_CAR_DIRT;
            buffalo.MarkAsNoLongerNeeded();
        }

        private static void SetFreemodeClothes()
        {
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_ACCESSORIES, Constants.FREEMODE_ACCESSORIES_DRAWABLE, 0, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_FACE, Constants.FREEMODE_FACE_DRAWABLE, 0, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_HAIR, Constants.FREEMODE_HAIR_DRAWABLE, Constants.FREEMODE_HAIR_TEXTURE, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_TORSO, Constants.FREEMODE_TORSO_DRAWABLE, 0, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_LEGS, Constants.FREEMODE_LEGS_DRAWABLE, Constants.FREEMODE_LEGS_TEXTURE, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_FEET, Constants.FREEMODE_FEET_DRAWABLE, 0, 0);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _freemode, Constants.FREEMODE_COMPONENT_TORSO2, Constants.FREEMODE_TORSO2_DRAWABLE, 0, 0);
            Function.Call(Hash.SET_PED_PROP_INDEX, _freemode, Constants.FREEMODE_PROP_INDEX, Constants.FREEMODE_PROP_DRAWABLE, Constants.FREEMODE_PROP_TEXTURE, Constants.FREEMODE_PROP_ATTACH);
        }

        internal static Result<bool> TrevorAttackFreemode()
        {
            if (_freemode == null) return new Err<bool>("Freemode no existe.");
            if (_trevor == null) return new Err<bool>("Trevor no existe.");
            _freemode.Health = 1;
            _trevor.Task.Combat(_freemode);
            return new Ok<bool>(true);
        }

        internal static void CharacterCurse(PedHash hash, string insult = Constants.DEFAULT_CURSE_SPEECH)
        {
            switch (hash)
            {
                case PedHash.Michael:
                    if (_michael != null)
                        _michael.PlayAmbientSpeech(insult, "MICHAEL_NORMAL", SpeechModifier.Force);
                    break;
                case PedHash.Franklin:
                    if (_franklin != null)
                        _franklin.PlayAmbientSpeech(insult, "FRANKLIN_NORMAL", SpeechModifier.Force);
                    break;
                case PedHash.Trevor:
                    if (_trevor != null)
                        _trevor.PlayAmbientSpeech(insult, "TREVOR_NORMAL", SpeechModifier.Force);
                    break;
                case PedHash.FreemodeMale01:
                    if (_freemode != null)
                        _freemode.Task.PlayAnimation(Constants.FREEMODE_CURSE_ANIM_DICT, Constants.FREEMODE_CURSE_ANIM_NAME);
                    break;
            }
        }

        internal static Result<bool> TrevorFight()
        {
            if (_freemode == null) return new Err<bool>("Freemode no existe.");
            if (_trevor == null) return new Err<bool>("Trevor no existe.");
            _trevor.Task.Combat(_freemode);
            return new Ok<bool>(true);
        }
    }
}