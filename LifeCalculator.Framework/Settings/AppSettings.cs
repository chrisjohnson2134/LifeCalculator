using LifeCalculator.Framework.Accounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LifeCalculator.Framework.Settings
{
    public class AppSettings
    {
        private static string userSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plaid Demo");
        private static string settingsPath = Path.Combine(userSettings, "Settings.xml");

        #region Singleton
        public static AppSettings Instance = new AppSettings();
        #endregion Singleton

        #region Settings
        public List<Institution> SandboxInstitutions { get; set; }
        public List<Institution> DevelopmentInstitutions { get; set; }
        public PlaidSettings PlaidSettings { get; set; }

        private static AppSettings GetDefaultValues()
        {
            AppSettings defaultSettings = new AppSettings();

            defaultSettings.SandboxInstitutions = new List<Institution>();
            defaultSettings.DevelopmentInstitutions = new List<Institution>();

            defaultSettings.PlaidSettings = new PlaidSettings();

            return defaultSettings;
        }
        #endregion Settings

        public static List<Institution> GetCurrentInstitutions()
        {
            if (Instance.PlaidSettings.Environment == Enums.Environment.Sandbox)
                return Instance.SandboxInstitutions;
            else
                return Instance.DevelopmentInstitutions;
        }

        #region Save Settings
        public static void SaveSettings()
        {
            if (!Directory.Exists(userSettings))
                Directory.CreateDirectory(userSettings);

            TextWriter writer = new StreamWriter(settingsPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppSettings));
            xmlSerializer.Serialize(writer, Instance);
            writer.Close();
        }
        #endregion Save Settings

        #region Read Settings
        public static void ReadSettings()
        {
            try
            {
                XDocument document = XDocument.Load(settingsPath);
                using (FileStream fileStream = new FileStream(settingsPath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppSettings));
                    Instance = (AppSettings)xmlSerializer.Deserialize(fileStream);
                }

                foreach (PropertyInfo prop in typeof(AppSettings).GetProperties())
                {
                    if (prop.GetValue(Instance) == null) //Check to see if any of the properties are null
                        prop.SetValue(Instance, prop.GetValue(GetDefaultValues())); //Replace that value with the default value
                }
            }
            catch
            {
                Instance = GetDefaultValues();
            }
        }
        #endregion Read Settings
    }
}
