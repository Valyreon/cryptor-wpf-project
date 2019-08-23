using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the abstract class that every ViewModel in MVVM architecture must implement.
    /// </summary>
    public abstract class ViewModelBase : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        private readonly object lockObject = new object();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasErrors
        {
            get
            {
                return this.errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
            }
        }

        public void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (this.lockObject)
            {
                var validationContext = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (this.errors.ContainsKey(propertyName))
                {
                    this.errors.Remove(propertyName);
                }

                this.OnErrorsChanged(propertyName);
                this.HandleValidationResults(validationResults);
            }
        }

        public void Validate()
        {
            lock (this.lockObject)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = this.errors.Keys.ToList();
                this.errors.Clear();
                propNames.ForEach(pn => this.OnErrorsChanged(pn));
                this.HandleValidationResults(validationResults);
            }
        }

        public IEnumerable GetErrors(string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (this.errors.ContainsKey(propertyName) && (this.errors[propertyName] != null) && this.errors[propertyName].Count > 0)
                {
                    return this.errors[propertyName].ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return this.errors.SelectMany(err => err.Value.ToList());
            }
        }

        public void ClearErrors()
        {
            this.errors.Clear();
        }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                this.errors.Add(prop.Key, messages);
                this.OnErrorsChanged(prop.Key);
            }
        }
    }
}
