using System.Drawing;
using GTA;

namespace MMI_SP.Helpers
{
    internal static class Sprite
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Image _cachedSprite;
        private static string _cachedFileName;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void InsuranceStatus(string fileName, float x, float y, Color color)
        {
            if (_cachedSprite == null || _cachedFileName != fileName)
            {
                _cachedSprite = Image.FromFile(fileName);
                _cachedFileName = fileName;
            }

            GTA.UI.CustomSprite a = new GTA.UI.CustomSprite(fileName, new Size(_cachedSprite.Width, _cachedSprite.Height), new PointF(x, y), color, 0.0f);
            a.Draw();
        }
    }
}