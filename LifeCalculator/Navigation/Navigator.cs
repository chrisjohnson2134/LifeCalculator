using LifeCalculator.Commands;
using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.CurrentAccountStorage;
using LifeCalculator.Framework.Enums;
using LifeCalculator.ViewModels.Factory;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows.Input;

namespace LifeCalculator.Navigation
{
    public class Navigator : INavigator
    {
        #region Events

        public event Action StateChanged;

        #endregion

        #region Fields

        private ViewModelBase _currentViewModel;

        #endregion

        #region Constructors

        public Navigator( IViewModelFactory viewModelFactory)
        {
        }

        #endregion

        #region Properties

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                StateChanged?.Invoke();
            }
        }

        #endregion
    }
}
