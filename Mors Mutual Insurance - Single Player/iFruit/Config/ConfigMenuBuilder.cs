using System;
using System.Collections.Generic;
using MMI_SP.Config;
using MMI_SP.Helpers;
using NativeUI;

namespace MMI_SP.iFruit
{
    internal static class ConfigMenuBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void AddCheckbox(UIMenu menu, string section, string key, bool value, string label)
        {
            var item = new UIMenuCheckboxItem(label, value, "");
            menu.AddItem(item);
            menu.OnCheckboxChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                Persistence.SetSetting(section, key, changed.Checked.ToString());
                ModSettings.UpdateValue(key, changed.Checked);
                Persistence.SaveSettings();
            };
        }

        internal static void AddListInt(UIMenu menu, string section, string key, int value, string label, int start, int stop, int step)
        {
            var values = new List<dynamic>();
            int counter = Constants.ZERO;
            bool found = false;
            for (int i = start; i <= stop; i += step)
            {
                values.Add(i);
                if (!found)
                {
                    if (value == i) found = true;
                    else counter++;
                }
            }
            var item = new UIMenuListItem(label, values, counter, "");
            menu.AddItem(item);
            menu.OnListChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                int val = (int)changed.Items[index];
                Persistence.SetSetting(section, key, val.ToString());
                ModSettings.UpdateValue(key, val);
                Persistence.SaveSettings();
            };
        }

        internal static void AddListFloat(UIMenu menu, string section, string key, float value, string label, float start, float stop, float step)
        {
            var values = new List<dynamic>();
            int counter = Constants.ZERO;
            bool found = false;
            for (float i = start; i <= stop; i += step)
            {
                float rounded = (float)Math.Round(i, Constants.ROUND_DECIMALS, MidpointRounding.AwayFromZero);
                values.Add(rounded);
                if (!found)
                {
                    if (Math.Abs(value - rounded) < Constants.FLOAT_EPSILON)
                        found = true;
                    else
                        counter++;
                }
            }
            var item = new UIMenuListItem(label, values, counter, "");
            menu.AddItem(item);
            menu.OnListChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                float val = (float)changed.Items[index];
                Persistence.SetSetting(section, key, val.ToString().Replace(",", "."));
                ModSettings.UpdateValue(key, val);
                Persistence.SaveSettings();
            };
        }

        internal static UIMenu AddSubMenu(MenuPool pool, UIMenu parent, string label)
        {
            var item = new UIMenuItem(label);
            parent.AddItem(item);
            var sub = new UIMenu("Configuración", label);
            pool.Add(sub);
            parent.BindMenuToItem(sub, item);
            return sub;
        }

        internal static void AddActionItem(UIMenu menu, string label, Action action)
        {
            var item = new UIMenuItem(label);
            menu.AddItem(item);
            menu.OnItemSelect += (sender, selectedItem, index) => { if (selectedItem == item) action?.Invoke(); };
        }
    }
}