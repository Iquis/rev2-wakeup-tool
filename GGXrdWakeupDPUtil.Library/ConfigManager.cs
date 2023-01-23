using System.Configuration;

namespace GGXrdWakeupDPUtil.Library
{
    public static class ConfigManager
    {

        public static void Set(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                LogManager.Instance.WriteLine("Error writing app settings");
                LogManager.Instance.WriteException(ex);
            }
        }
    }
}
