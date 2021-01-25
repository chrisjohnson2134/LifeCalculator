//keep track of project https://kanbanflow.com/board/eFu1Zwn

using LifeCalculator.Commands;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Navigation;
using LifeCalculator.ViewModels.Factory;
using System.Windows.Input;

namespace LifeCalculator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigator _navigator;

        #endregion

        #region Constructors

        public MainWindowViewModel(INavigator navigator, ViewModelFactory viewModelFactory)
        {
            _navigator = navigator;
            _navigator.StateChanged += Navigator_OnStateChanged;
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);
        }

        #endregion

        #region Properties

        public ViewModelBase CurrentViewModel => _navigator.CurrentViewModel;

        public ICommand UpdateCurrentViewModelCommand { get; private set; }

        #endregion

        #region Event Handlers

        private void Navigator_OnStateChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        #endregion
    }
}
