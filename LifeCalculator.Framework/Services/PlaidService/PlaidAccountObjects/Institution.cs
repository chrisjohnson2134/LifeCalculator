using LifeCalculator.Framework.Services.DataService;
using System.Collections.Generic;

namespace LifeCalculator.Framework.Accounts
{
    public class Institution
    {
        public Institution()
        {
            Credentials = new Credentials();
            Accounts = new List<Account>();
        }

        public string Name { get; set; }
        public string PlaidId { get; set; }
        public int Id { get; set; }
        [IgnoreDatabase]
        public Credentials Credentials { get; set; }
        public string AccessToken
        {
            get { return Credentials.AccessToken; }
            set { Credentials.AccessToken = value; }
        }
        public string Item
        {
            get { return Credentials.Item; }
            set { Credentials.Item = value; }
        }
        [IgnoreDatabase]
        public List<Account> Accounts { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
