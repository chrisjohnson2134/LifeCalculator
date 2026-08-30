using LifeCalculator.Control.ViewModels;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.Income;
using NUnit.Framework;
using Should;

namespace LifeCalculator.ControlTest.Accounts.Retirement
{
    [TestFixture]
    public class AddRetirementViewModelTest
    {
        private AddRetirementViewModel setupViewModel()
        {
            var accountStore = new AccountStore
            {
                CurrentAccount = new LifeCalculator.Framework.FinancialAccount.FinancialAccount()
            };

            return new AddRetirementViewModel(accountStore);
        }

        /// <summary>A gross salary job, so GrossAnnualSalary is derived straight from the pay rate.</summary>
        private IncomeStream salaryOf(double annual)
        {
            return new IncomeStream
            {
                Name = "Job",
                IsGross = true,
                PayFrequency = PayFrequency.Annual,
                PayRate = annual
            };
        }

        [Test]
        public void DefaultBasis_IsPercentOfSalary()
        {
            var vm = setupViewModel();

            vm.ContributionBasis.ShouldEqual(ContributionBasis.PercentOfSalary);
            vm.IsPercentBasis.ShouldBeTrue();
            vm.IsDollarBasis.ShouldBeFalse();
        }

        [Test]
        public void PercentEntered_DerivesMonthlyDollars()
        {
            var vm = setupViewModel();
            vm.LinkedIncomeStream = salaryOf(120000);

            vm.ContributionPercent = 6;

            // 120,000 / 12 = 10,000 a month; 6% of that is 600.
            vm.Contribute.ShouldEqual(600d);
        }

        [Test]
        public void DollarsEntered_DerivesPercent()
        {
            var vm = setupViewModel();
            vm.LinkedIncomeStream = salaryOf(120000);
            vm.ContributionBasis = ContributionBasis.DollarAmount;

            vm.Contribute = 750;

            vm.ContributionPercent.ShouldEqual(7.5d);
        }

        [Test]
        public void PercentBasis_ChangingJob_KeepsPercentAndRederivesDollars()
        {
            var vm = setupViewModel();
            vm.LinkedIncomeStream = salaryOf(120000);
            vm.ContributionPercent = 6;
            vm.Contribute.ShouldEqual(600d);

            vm.LinkedIncomeStream = salaryOf(60000);

            vm.ContributionPercent.ShouldEqual(6d);
            vm.Contribute.ShouldEqual(300d);
        }

        [Test]
        public void DollarBasis_ChangingJob_KeepsDollarsAndRederivesPercent()
        {
            var vm = setupViewModel();
            vm.ContributionBasis = ContributionBasis.DollarAmount;
            vm.LinkedIncomeStream = salaryOf(120000);
            vm.Contribute = 500;

            vm.LinkedIncomeStream = salaryOf(60000);

            vm.Contribute.ShouldEqual(500d);
            // 500 of a 5,000 monthly salary.
            vm.ContributionPercent.ShouldEqual(10d);
        }

        [Test]
        public void SwitchingBasis_DoesNotChangeEitherFigure()
        {
            var vm = setupViewModel();
            vm.LinkedIncomeStream = salaryOf(120000);
            vm.ContributionPercent = 6;

            vm.ContributionBasis = ContributionBasis.DollarAmount;

            vm.Contribute.ShouldEqual(600d);
            vm.ContributionPercent.ShouldEqual(6d);
        }

        [Test]
        public void NoLinkedJob_PercentCannotBeConverted_AndEquivalentTextIsHidden()
        {
            var vm = setupViewModel();

            vm.ContributionPercent = 6;

            vm.Contribute.ShouldEqual(0d);
            vm.ContributionEquivalentText.ShouldBeEmpty();
        }

        [Test]
        public void EquivalentText_ShowsTheOppositeFigureForTheSelectedBasis()
        {
            var vm = setupViewModel();
            vm.LinkedIncomeStream = salaryOf(120000);

            vm.ContributionPercent = 6;
            vm.ContributionEquivalentText.ShouldContain("600");

            vm.ContributionBasis = ContributionBasis.DollarAmount;
            vm.ContributionEquivalentText.ShouldContain("6");
            vm.ContributionEquivalentText.ShouldContain("%");
        }

        [Test]
        public void PercentOutOfRange_ProducesError()
        {
            var vm = setupViewModel();

            vm.ContributionPercent = 150;

            vm.GetErrors(nameof(vm.ContributionPercent)).ShouldNotBeNull();
        }

        [Test]
        public void NegativeDollarAmount_ProducesError()
        {
            var vm = setupViewModel();

            vm.Contribute = -50;

            vm.GetErrors(nameof(vm.Contribute)).ShouldNotBeNull();
        }

        /// <summary>
        /// The derived side is written to its backing field rather than through its setter, so its
        /// rule has to be re-run on a salary change or a stale pass would linger.
        /// </summary>
        [Test]
        public void DollarBasis_SalaryDropMakingDerivedPercentExceed100_ProducesError()
        {
            var vm = setupViewModel();
            vm.ContributionBasis = ContributionBasis.DollarAmount;
            vm.LinkedIncomeStream = salaryOf(120000);
            vm.Contribute = 500;
            vm.GetErrors(nameof(vm.ContributionPercent)).ShouldBeNull();

            // 500 a month against a 300/month salary is over 100%.
            vm.LinkedIncomeStream = salaryOf(3600);

            vm.GetErrors(nameof(vm.ContributionPercent)).ShouldNotBeNull();
        }
    }
}
