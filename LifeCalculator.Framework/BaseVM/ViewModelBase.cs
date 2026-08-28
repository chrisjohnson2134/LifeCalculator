using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeCalculator.Framework.BaseVM
{

    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;

    public class ViewModelBase : ObservableObject
    {
    }
}
