using GTA;

namespace GTAVStudio.Common
{
    public static class StudioSettings
    {
        private const string SettingsPath = "scripts//GTAVStudio.ini";
        private static ScriptSettings _scriptSettings = ScriptSettings.Load(SettingsPath);

        public static T GetValue<T>(string section, string name, T defaultvalue)
            => _scriptSettings.GetValue(section, name, defaultvalue);

        public static void Reload()
        {
            _scriptSettings = ScriptSettings.Load(SettingsPath);
        }
    }
}