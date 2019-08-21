using FileEncryptorWpf.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FileEncryptorWpf.ViewModels
{
    public class ViewModelBase : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private readonly object _lock = new object();
        public bool HasErrors { get { return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName))
                    _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = _errors.Keys.ToList();
                _errors.Clear();
                propNames.ForEach(pn => OnErrorsChanged(pn));
                HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }

        public IEnumerable GetErrors(string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }
    }
}
