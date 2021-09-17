namespace LifeCalculator.Framework.Settings
{
    public class PlaidSettings
    {
        public string Client_Id { get; set; }
        public string Public_Key { get; set; }
        public string Sandbox_Secret { get; set; }
        public string Development_Secret { get; set; }
        public Enums.Environment Environment { get; set; }

    }
}
