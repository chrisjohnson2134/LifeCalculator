using LifeCalculator.Framework.BaseVM;
using System;
using System.Windows.Media;

namespace LifeCalculator.ViewModels
{
    /// <summary>
    /// One entry in the chart legend. Clicking it shows or hides that account's line, which is
    /// the only practical way to read a chart once someone has half a dozen accounts overlapping.
    /// Carries its own swatch colour so the legend and the plotted line can't drift apart.
    /// </summary>
    public class ChartSeriesToggle : ViewModelBase
    {
        private readonly Action _onChanged;

        public ChartSeriesToggle(string name, Brush color, bool isVisible, Action onChanged)
        {
            Name = name;
            Color = color;
            _isVisible = isVisible;
            _onChanged = onChanged;
        }

        public string Name { get; }

        public Brush Color { get; }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
                _onChanged?.Invoke();
            }
        }
    }
}
