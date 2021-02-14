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

        #endregion

        #region Constructors

        public ViewModelFactory(CreateViewModel<HomeViewModel> createHomeViewModel, CreateViewModel<FinancialProfileViewModel> createFinancialProfileViewModel,
            CreateViewModel<LoanProfileViewModel> createLoanProfileViewModel, CreateViewModel<LoginViewModel> createLoginViewModel,
            CreateViewModel<RegisterViewModel> createRegisterViewModel, CreateViewModel<WelcomePageViewModel> createWelcomePageViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
            _createFinancialProfileViewModel = createFinancialProfileViewModel;
            _createLoanProfileViewModel = createLoanProfileViewModel;
            _createLoginViewModel = createLoginViewModel;
            _createRegisterViewModel = createRegisterViewModel;
            _createWelcomePageViewModel = createWelcomePageViewModel;
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
                case ViewType.Login:
                    return _createLoginViewModel();
                case ViewType.Register:
                    return _createRegisterViewModel();
                case ViewType.Welcome:
                    return _createWelcomePageViewModel();
                default:
                    throw new ArgumentException("This view type does not have a view model.");
            }
        }

        #endregion
    }
}
