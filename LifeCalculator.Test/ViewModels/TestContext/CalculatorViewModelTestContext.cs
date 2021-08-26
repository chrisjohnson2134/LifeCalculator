using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Test.ViewModels.TestContext
{
    public class CalculatorViewModelTestContext
    {
        public CalculatorViewModel CreateBasicCalculatorViewModel()
        {
            return new CalculatorViewModel(CreateAccountStore());
        }

        public AccountStore CreateAccountStore()
        {
            var output = new AccountStore();
            return output;
        }

        public CompoundAccount CreateBasicCompoundAccount()
        {
            var output = new CompoundAccount()
            {
                Name = "basic compound accout",
                InitialAmount = 1000,
                MonthlyContribute = 400,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(5),
                InterestRate = 10
            };

            return output;
        }



    }
}
