using LifeCalculator.Framework.BaseVM;

namespace LifeCalculator.Tools.Common.Messages
{
    public class ViewModelMessage : ViewModelBase
    {

        #region Fields

        private string _message;

        #endregion

        #region Properties

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(HasMessage));
            }
        }

        public bool HasMessage => !string.IsNullOrEmpty(Message);

        #endregion
    }
}
