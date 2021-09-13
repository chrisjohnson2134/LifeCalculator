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
        public string Id { get; set; }
        public Credentials Credentials { get; set; }
        public List<Account> Accounts { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
