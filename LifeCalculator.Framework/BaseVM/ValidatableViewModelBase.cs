using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace LifeCalculator.Framework.BaseVM
{
    public abstract class ValidatableViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Re-evaluates the command's CanExecute (e.g. an "Add"/"Save" button gated on !HasErrors)
        /// whenever this view model's validation state changes.
        /// </summary>
        protected void LinkCommandToValidation(IRelayCommand command)
        {
            ErrorsChanged += (s, e) => command.NotifyCanExecuteChanged();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return null;

            return _errors[propertyName];
        }

        /// <summary>
        /// Adds or removes only this rule's own error message for the property, leaving any
        /// other rules' errors on the same property untouched (a property can have several
        /// independent rules, e.g. DownPayment >= 0 and DownPayment &lt;= LoanAmount).
        /// </summary>
        protected void Validate(string propertyName, Func<bool> isValid, string error)
        {
            if (isValid())
                RemoveError(propertyName, error);
            else
                SetError(propertyName, error);
        }

        protected void SetError(string propertyName, string error)
        {
            if (!_errors.TryGetValue(propertyName, out var errorList))
            {
                errorList = new List<string>();
                _errors[propertyName] = errorList;
            }

            if (!errorList.Contains(error))
            {
                errorList.Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }

        protected void RemoveError(string propertyName, string error)
        {
            if (_errors.TryGetValue(propertyName, out var errorList) && errorList.Remove(error))
            {
                if (errorList.Count == 0)
                    _errors.Remove(propertyName);

                RaiseErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }
}
