using LifeCalculator.Framework.BaseVM;
using System;

namespace LifeCalculator.Navigation
{
    public interface INavigator
    {
        ViewModelBase CurrentViewModel { get; set; }

        event Action StateChanged;
    }
}
