using LifeCalculator.Framework.Settings;
using NUnit.Framework;
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.FrameworkTest.Services.PlaidService
{
    [TestFixture]
    public class AppSettingsTest
    {
        protected string _clientIDExpected = "clientIDExpected";
        protected string _publicKeyExpected = "publicKeyExpected";
        protected string _sandboxSecretExpected = "sandboxSecretExpected";
        protected string _developmentSecretExpected = "developmentSecretExpected";
        protected Environment _environmentExpected = Environment.Sandbox;


        [OneTimeSetUp]
        public void Setup()
        {
            AppSettings.ReadSettings();

            AppSettings.Instance.PlaidSettings.Client_Id = _clientIDExpected;
            AppSettings.Instance.PlaidSettings.Public_Key = _publicKeyExpected;
            AppSettings.Instance.PlaidSettings.Sandbox_Secret = _sandboxSecretExpected;
            AppSettings.Instance.PlaidSettings.Development_Secret = _developmentSecretExpected;
            AppSettings.Instance.PlaidSettings.Environment = _environmentExpected;
        }

        [Test]
        public void SaveAndLoadSettings()
        {
            AppSettings.SaveSettings();

            AppSettings.Instance.PlaidSettings.Client_Id = string.Empty;
            AppSettings.Instance.PlaidSettings.Public_Key = string.Empty;
            AppSettings.Instance.PlaidSettings.Sandbox_Secret = string.Empty;
            AppSettings.Instance.PlaidSettings.Development_Secret = string.Empty;

            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Client_Id, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Public_Key, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Sandbox_Secret, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Development_Secret, string.Empty);

            AppSettings.ReadSettings();

            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Client_Id, _clientIDExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Public_Key, _publicKeyExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Sandbox_Secret, _sandboxSecretExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Development_Secret, _developmentSecretExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidSettings.Environment, _environmentExpected);

        }
    }
}
