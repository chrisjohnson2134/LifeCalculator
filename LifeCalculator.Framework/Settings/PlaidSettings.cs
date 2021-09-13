namespace LifeCalculator.Framework.Settings
{
    public class PlaidSettings
    {
        #region Singleton
        public static PlaidSettings Instance = new PlaidSettings();
        #endregion Singleton

        public string Client_Id { get; set; }
        public string Public_Key { get; set; }
        public string Sandbox_Secret { get; set; }
        public string Development_Secret { get; set; }
        public Enums.Environment Environment { get; set; }

    }
}
