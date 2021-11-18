using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using LifeCalculator.Framework.Accounts;
using Newtonsoft.Json;
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

        public AppSettings()
        {
            SandboxInstitutions = new List<Institution>();
            DevelopmentInstitutions = new List<Institution>();
        }

        #region Settings
        public List<Institution> SandboxInstitutions { get; set; }
        public List<Institution> DevelopmentInstitutions { get; set; }
        public PlaidSettings PlaidApiSettings { get; set; }

        //private static AppSettings GetDefaultValues()
        //{
        //    AppSettings defaultSettings = new AppSettings();

        //    defaultSettings.SandboxInstitutions = new List<Institution>();
        //    defaultSettings.DevelopmentInstitutions = new List<Institution>();

        //    defaultSettings.PlaidApiSettings = new PlaidSettings();

        //    return defaultSettings;
        //}
        #endregion Settings

        public static List<Institution> GetCurrentInstitutions()
        {
            if (Instance.PlaidApiSettings.Environment == Enums.Environment.Sandbox)
                return Instance.SandboxInstitutions;
            else
                return Instance.DevelopmentInstitutions;
        }

        #region Save Settings
        //public static void SaveSettings()
        //{
        //    if (!Directory.Exists(userSettings))
        //        Directory.CreateDirectory(userSettings);

        //    TextWriter writer = new StreamWriter(settingsPath);
        //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppSettings));
        //    xmlSerializer.Serialize(writer, Instance);
        //    writer.Close();
        //}
        #endregion Save Settings

        #region Read Settings
        //public static void ReadSettings()
        //{
        //    try
        //    {
        //        XDocument document = XDocument.Load(settingsPath);
        //        using (FileStream fileStream = new FileStream(settingsPath, FileMode.Open))
        //        {
        //            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppSettings));
        //            Instance = (AppSettings)xmlSerializer.Deserialize(fileStream);
        //        }

        //        foreach (PropertyInfo prop in typeof(AppSettings).GetProperties())
        //        {
        //            if (prop.GetValue(Instance) == null) //Check to see if any of the properties are null
        //                prop.SetValue(Instance, prop.GetValue(GetDefaultValues())); //Replace that value with the default value
        //        }
        //    }
        //    catch
        //    {
        //        Instance = GetDefaultValues();
        //    }
        //}
        #endregion Read Settings

        public static void LoadCredentials()
        {
            GetSecret();
        }

        private static void GetSecret()
        {
            string secretName = "PlaidApiKeys";
            string region = "us-east-1";
            string secret = "";

            MemoryStream memoryStream = new MemoryStream();

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
        

            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response = null;

            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
                PlaidSettings plaidApiSettings = JsonConvert.DeserializeObject<PlaidSettings>(response.SecretString);
                plaidApiSettings.Environment = Enums.Environment.Development;
                Instance.PlaidApiSettings = plaidApiSettings;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }
        }

    }
}
