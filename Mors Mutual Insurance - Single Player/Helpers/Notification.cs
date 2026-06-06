using GTA.Native;

namespace MMI_SP.Helpers
{
    internal static class Notification
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void Show(string title, string message, string description)
        {
            string text = string.IsNullOrEmpty(description)
                ? $"{title}: {message}"
                : $"{title}: {message} {description}";

            GTA.UI.Notification.PostTicker(text, isImportant: false);
        }

        public static void ShowMMI(string subject, string message)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_THEFEED_POST, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, message);  // cuerpo del mensaje (p)
            Function.Call(Hash.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT,
                "char_mp_mors_mutual",   // txd del icono
                "char_mp_mors_mutual",   // textura del icono
                false,                   // flash: parpadea la notificación
                Constants.NOTIFY_ICON_NONE, // iconType: activa jerarquía sender+subject+body
                "MORS MUTUAL INSURANCE", // sender (arriba)
                subject);                // subject en h1 (debajo del sender)
        }

        public static void ShowMechanic(string subject, string message)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_THEFEED_POST, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, message);  // cuerpo del mensaje (p)
            Function.Call(Hash.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT,
                "char_ls_customs",       // txd del icono
                "char_ls_customs",       // textura del icono
                false,                   // flash: parpadea la notificación
                Constants.NOTIFY_ICON_MAIL, // iconType: activa jerarquía sender+subject+body
                "LSC",                   // sender (arriba)
                subject);                // subject en h1 (debajo del sender)
        }

        public static void ShowiFruit(string subject, string message)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_THEFEED_POST, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, message);  // cuerpo del mensaje (p)
            Function.Call(Hash.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT,
                "char_mp_fm_contact",   // txd del icono (en minúsculas)
                "char_mp_fm_contact",   // textura del icono (en minúsculas)
                false,                  // flash: parpadea la notificación
                Constants.NOTIFY_ICON_MESSAGE, // iconType: activa jerarquía sender+subject+body
                "Configuración",        // sender (arriba)
                subject);               // subject en h1 (debajo del sender)
        }
    }
}