using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Navigation
{
    public interface INavigator
    {
        ViewModelBase CurrentViewModel { get; set; }
        ViewType CurrentViewType { get; set; }

        event Action StateChanged;
    }
}
