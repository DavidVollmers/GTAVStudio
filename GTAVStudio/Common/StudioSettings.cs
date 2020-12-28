using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;

namespace GTAVStudio.Common
{
    public static class StudioSettings
    {
        private const string SettingsPath = "scripts//GTAVStudio.ini";
        private static ScriptSettings _scriptSettings = ScriptSettings.Load(SettingsPath);

        public static Keys GetShortcut(string name, Keys defaultvalue)
        {
            var result = _scriptSettings.GetValue<string>(Constants.Settings.Shortcuts, name, null);
            if (result == null) return defaultvalue;
            var keys = result.Split('+');
            var shortcut = Keys.None;
            foreach (var key in keys)
            {
                if (Enum.TryParse<Keys>(key, out var shortcutKey))
                {
                    shortcut |= shortcutKey;
                }
                else
                {
                    return defaultvalue;
                }
            }

            return shortcut == Keys.None ? defaultvalue : shortcut;
        }

        public static T GetValue<T>(string section, string name, T defaultvalue)
        {
            try
            {
                if (typeof(T) != typeof(Color)) return _scriptSettings.GetValue(section, name, defaultvalue);

                var intResult = _scriptSettings.GetValue(section, name, -1);
                if (intResult != -1) return (T) Convert.ChangeType(Color.FromArgb(intResult), typeof(T));

                var strResult = _scriptSettings.GetValue<string>(section, name, null);
                if (strResult == null) return defaultvalue;
                return (T) Convert.ChangeType(Color.FromName(strResult), typeof(T));
            }
            catch (Exception)
            {
                return defaultvalue;
            }
        }

        public static void Reload()
        {
            _scriptSettings = ScriptSettings.Load(SettingsPath);
        }
    }
}