using LifeCalculator.Control.ViewModels;
using NUnit.Framework;
using Should;

namespace LifeCalculator.ControlTest.Accounts.Loan
{
    [TestFixture]
    public class AddLoanViewModelTest
    {
        private AddLoanViewModel setupViewModel()
        {
            return new AddLoanViewModel();
        }

        [Test]
        public void DefaultState_HasErrors()
        {
            var vm = setupViewModel();

            vm.HasErrors.ShouldBeTrue();
            vm.AddAccountCommand.CanExecute(null).ShouldBeFalse();
        }

        [Test]
        public void LoanLength_Zero_ProducesError()
        {
            var vm = setupViewModel();

            vm.LoanLength = 0;

            vm.GetErrors(nameof(vm.LoanLength)).ShouldNotBeNull();
        }

        [Test]
        public void LoanLength_Positive_ClearsError()
        {
            var vm = setupViewModel();

            vm.LoanLength = 30;

            vm.GetErrors(nameof(vm.LoanLength)).ShouldBeNull();
        }

        [Test]
        public void InterestRate_OutOfRange_ProducesError()
        {
            var vm = setupViewModel();

            vm.InterestRate = 150;

            vm.GetErrors(nameof(vm.InterestRate)).ShouldNotBeNull();

            vm.InterestRate = -5;

            vm.GetErrors(nameof(vm.InterestRate)).ShouldNotBeNull();
        }

        [Test]
        public void InterestRate_InRange_ClearsError()
        {
            var vm = setupViewModel();

            vm.InterestRate = 4.5;

            vm.GetErrors(nameof(vm.InterestRate)).ShouldBeNull();
        }

        [Test]
        public void DownPayment_ExceedsLoanAmount_ProducesError()
        {
            var vm = setupViewModel();

            vm.InitialLoanAmount = 10000;
            vm.DownPayment = 20000;

            vm.GetErrors(nameof(vm.DownPayment)).ShouldNotBeNull();
        }

        [Test]
        public void DownPayment_WithinLoanAmount_ClearsError()
        {
            var vm = setupViewModel();

            vm.InitialLoanAmount = 10000;
            vm.DownPayment = 2000;

            vm.GetErrors(nameof(vm.DownPayment)).ShouldBeNull();
        }

        [Test]
        public void DownPayment_ThenLoanAmountLowered_RevalidatesDownPayment()
        {
            var vm = setupViewModel();

            vm.InitialLoanAmount = 10000;
            vm.DownPayment = 8000;
            vm.GetErrors(nameof(vm.DownPayment)).ShouldBeNull();

            vm.InitialLoanAmount = 5000;

            vm.GetErrors(nameof(vm.DownPayment)).ShouldNotBeNull();
        }

        [Test]
        public void AccountName_Empty_ProducesError()
        {
            var vm = setupViewModel();

            vm.AccountName = "";

            vm.GetErrors(nameof(vm.AccountName)).ShouldNotBeNull();
        }

        [Test]
        public void AllFieldsValid_HasErrorsFalse_CommandCanExecute()
        {
            var vm = setupViewModel();

            vm.AccountName = "Car Loan";
            vm.InitialLoanAmount = 25000;
            vm.InterestRate = 5.5;
            vm.LoanLength = 5;
            vm.DownPayment = 2000;

            vm.HasErrors.ShouldBeFalse();
            vm.AddAccountCommand.CanExecute(null).ShouldBeTrue();
        }

        [Test]
        public void MultipleIndependentRulesOnSameProperty_DoNotClobberEachOther()
        {
            var vm = setupViewModel();

            vm.InitialLoanAmount = 10000;
            vm.DownPayment = -5;

            // Negative down payment should still be flagged even though
            // -5 <= 10000 (the "exceeds loan amount" rule would pass).
            vm.GetErrors(nameof(vm.DownPayment)).ShouldNotBeNull();
        }
    }
}
