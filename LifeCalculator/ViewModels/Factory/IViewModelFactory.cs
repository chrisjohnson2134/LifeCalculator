using LifeCalculator.Framework.BaseVM;
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.ViewModels.Factory
{
    public interface IViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewType viewType);   
    }
}
