using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.ViewModels.Factory
{
    public class ViewModelFactory : IViewModelFactory
    {
        #region Fields

        private readonly CreateViewModel<HomeViewModel> _createHomeViewModel;
        private readonly CreateViewModel<FinancialProfileViewModel> _createFinancialProfileViewModel;
        private readonly CreateViewModel<LoanProfileViewModel> _createLoanProfileViewModel;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;
        private readonly CreateViewModel<RegisterViewModel> _createRegisterViewModel;
        private readonly CreateViewModel<WelcomePageViewModel> _createWelcomePageViewModel;
        private readonly CreateViewModel<BudgetViewModel> _createBudgetViewModel;
        private readonly CreateViewModel<CalculatorViewModel> _createCalculatorPageViewModel;

        #endregion

        #region Constructors

        public ViewModelFactory(CreateViewModel<HomeViewModel> createHomeViewModel, CreateViewModel<FinancialProfileViewModel> createFinancialProfileViewModel,
            CreateViewModel<LoanProfileViewModel> createLoanProfileViewModel, CreateViewModel<LoginViewModel> createLoginViewModel,
            CreateViewModel<RegisterViewModel> createRegisterViewModel, CreateViewModel<WelcomePageViewModel> createWelcomePageViewModel,
            CreateViewModel<BudgetViewModel> createBudgetViewModel,CreateViewModel<CalculatorViewModel> calculatorViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
            _createFinancialProfileViewModel = createFinancialProfileViewModel;
            _createLoanProfileViewModel = createLoanProfileViewModel;
            _createLoginViewModel = createLoginViewModel;
            _createBudgetViewModel = createBudgetViewModel;
            _createRegisterViewModel = createRegisterViewModel;
            _createWelcomePageViewModel = createWelcomePageViewModel;
            _createCalculatorPageViewModel = calculatorViewModel;
        }

        #endregion

        #region Methods

        public ViewModelBase CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Home:
                    return _createHomeViewModel();
                case ViewType.FinancialProfile:
                    return _createFinancialProfileViewModel();
                case ViewType.LoanProfile:
                    return _createLoanProfileViewModel();
                case ViewType.Budget:
                    return _createBudgetViewModel();
                case ViewType.Login:
                    return _createLoginViewModel();
                case ViewType.Register:
                    return _createRegisterViewModel();
                case ViewType.Welcome:
                    return _createWelcomePageViewModel();
                case ViewType.Calculator:
                    return _createCalculatorPageViewModel();
                default:
                    throw new ArgumentException("This view type does not have a view model.");
            }
        }

        #endregion
    }
}
