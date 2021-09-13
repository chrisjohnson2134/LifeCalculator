using LifeCalculator.Framework.Settings;
using LifeCalculator.Framework.Enums;
using NUnit.Framework;

namespace LifeCalculator.FrameworkTest.Services.PlaidService
{
    [TestFixture]
    public class PlaidSettingsTest
    {
        #region Setup

        protected string _clientIDExpected = "clientIDExpected";
        protected string _publicKeyExpected = "publicKeyExpected";
        protected string _sandboxSecretExpected = "sandboxSecretExpected";
        protected string _developmentSecretExpected = "developmentSecretExpected";
        protected Environment _environmentExpected = Environment.Sandbox;

        [OneTimeSetUp]
        public void Setup()
        {
            PlaidSettings.Instance.Client_Id = _clientIDExpected;
            PlaidSettings.Instance.Public_Key = _publicKeyExpected;
            PlaidSettings.Instance.Sandbox_Secret = _sandboxSecretExpected;
            PlaidSettings.Instance.Development_Secret = _developmentSecretExpected;
            PlaidSettings.Instance.Environment = _environmentExpected;
        }

        #endregion

        #region Tests

        [Test]
        public void PlaidSettingsAreStatic()
        {
            Assert.AreEqual(PlaidSettings.Instance.Client_Id, _clientIDExpected);
            Assert.AreEqual(PlaidSettings.Instance.Public_Key, _publicKeyExpected);
            Assert.AreEqual(PlaidSettings.Instance.Sandbox_Secret, _sandboxSecretExpected);
            Assert.AreEqual(PlaidSettings.Instance.Development_Secret, _developmentSecretExpected);
            Assert.AreEqual(PlaidSettings.Instance.Environment, _environmentExpected);
        }

        #endregion

    }
}
