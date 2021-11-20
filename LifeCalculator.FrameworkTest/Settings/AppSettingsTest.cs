using LifeCalculator.Framework.Settings;
using NUnit.Framework;
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.FrameworkTest.Settings
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
            //AppSettings.ReadSettings();

            AppSettings.Instance.PlaidApiSettings.ClientId = _clientIDExpected;
            AppSettings.Instance.PlaidApiSettings.PublicKey = _publicKeyExpected;
            AppSettings.Instance.PlaidApiSettings.SandboxSecret = _sandboxSecretExpected;
            AppSettings.Instance.PlaidApiSettings.SecretKey = _developmentSecretExpected;
            AppSettings.Instance.PlaidApiSettings.Environment = _environmentExpected;
        }

        [Test]
        public void SaveAndLoadSettings()
        {
            //AppSettings.SaveSettings();

            AppSettings.Instance.PlaidApiSettings.ClientId = string.Empty;
            AppSettings.Instance.PlaidApiSettings.PublicKey = string.Empty;
            AppSettings.Instance.PlaidApiSettings.SandboxSecret = string.Empty;
            AppSettings.Instance.PlaidApiSettings.SecretKey = string.Empty;

            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.ClientId, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.PublicKey, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.SandboxSecret, string.Empty);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.SecretKey, string.Empty);

            //AppSettings.ReadSettings();

            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.ClientId, _clientIDExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.PublicKey, _publicKeyExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.SandboxSecret, _sandboxSecretExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.SecretKey, _developmentSecretExpected);
            Assert.AreEqual(AppSettings.Instance.PlaidApiSettings.Environment, _environmentExpected);

        }
    }
}
