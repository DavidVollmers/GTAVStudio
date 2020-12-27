using GTA;

namespace GTAVStudio.Common
{
    public static class StudioTranslations
    {
        private static string _settingsPath =
            $"scripts//GTAVStudio.{StudioSettings.GetValue(Constants.Settings.Overlay, "Culture", "en-US")}.ini";

        private static ScriptSettings _scriptSettings = ScriptSettings.Load(_settingsPath);

        public static T GetValue<T>(string section, string name, T defaultvalue)
            => _scriptSettings.GetValue(section, name, defaultvalue);

        public static void Reload()
        {
            _settingsPath =
                $"scripts//GTAVStudio.{StudioSettings.GetValue(Constants.Settings.Overlay, "Culture", "en-US")}.ini";
            _scriptSettings = ScriptSettings.Load(_settingsPath);
        }
    }
}