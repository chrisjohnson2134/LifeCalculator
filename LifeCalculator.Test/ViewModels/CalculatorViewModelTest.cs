using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.Account;
using LifeCalculator.Test.ViewModels.TestContext;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Test.ViewModels
{
    [TestFixture]
    public class CalculatorViewModelTest : CalculatorViewModelTestContext
    {

        [Test]
        public void AddAccountTest()
        {
            var testObject = CreateBasicCalculatorViewModel();

            testObject.AccountType = "Add Compound";

            (testObject.CurrentViewModel as AddCompoundViewModel).AccountName = "basic Compound";
            (testObject.CurrentViewModel as AddCompoundViewModel).DescriptionText = "description";
            (testObject.CurrentViewModel as AddCompoundViewModel).InitialValue = 1000;
            (testObject.CurrentViewModel as AddCompoundViewModel).Contribute = 200;
            (testObject.CurrentViewModel as AddCompoundViewModel).StopDate = DateTime.Now.AddYears(5);

            (testObject.CurrentViewModel as AddCompoundViewModel).AddAccountCommand.Execute(new object());

            Assert.That(testObject.AccountsList.Count == 1);
        }
    }
}
