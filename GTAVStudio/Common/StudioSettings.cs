using System;
using System.Drawing;
using GTA;

namespace GTAVStudio.Common
{
    public static class StudioSettings
    {
        private const string SettingsPath = "scripts//GTAVStudio.ini";
        private static ScriptSettings _scriptSettings = ScriptSettings.Load(SettingsPath);

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