using LifeCalculator.Framework.BaseVM;
using System;

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

        public Navigator()
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
