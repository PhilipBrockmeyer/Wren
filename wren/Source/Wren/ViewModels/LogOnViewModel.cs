using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using Wren.Core.Services;
using Wren.Core;
using Wren.Core.Settings;
using Wren.Core.Events;

namespace Wren.ViewModels
{
    public class LogOnViewModel : INotifyPropertyChanged
    {
        Boolean _successfulLogOn;
        String _userId;

        public event PropertyChangedEventHandler PropertyChanged;

        IEventAggregator _eventAggregator;
        ISettingsManager _settingsManager;

        public LogOnViewModel(IEventAggregator eventAggregator, ISettingsManager settingsManager)
        {
            _eventAggregator = eventAggregator;
            _settingsManager = settingsManager;

            var settings = _settingsManager.LoadSettings<LogOnSettings>(EmulationContext.Empty);
            Password = settings.Password;
            UserName = settings.Username;            
        }

        private String _UserName;
        public String UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        private String _Password;

        public String Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        private String _Email;

        public String Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                OnPropertyChanged("Email");
            }
        }

        private String _Error;

        public String Error
        {
            get { return _Error; }
            set
            {
                _Error = value;
                OnPropertyChanged("Error");
            }
        }        

        private Boolean _isLoading;
        public Boolean IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public void RegisterUser(String password)
        {
            if (IsLoading)
                return;

            IsLoading = true;

            Password = password;
            
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(Register_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LogOn_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }

        void LogOn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_successfulLogOn)
            {
                _eventAggregator.Publish(new LoggedOnEvent(_userId));
            }
        }

        public void LogOn(String password)
        {
            if (IsLoading)
                return;

            IsLoading = true;

            Password = password;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork +=new DoWorkEventHandler(LogOn_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LogOn_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }
        
        void LogOn_DoWork(object sender, DoWorkEventArgs e)
        {
            var svc = new AccountService();

            var result = svc.LogOn(UserName, Password);

            if (result == null)
            {
                WrenCore.IsOffline = true;
                var settings = _settingsManager.LoadSettings<LogOnSettings>(EmulationContext.Empty);

                _successfulLogOn = true;

                if (settings == null)
                {
                    Error = "A connection to the server could not be made.";
                    _successfulLogOn = false;
                }

                if (String.IsNullOrEmpty(settings.UserId))
                {
                    Error = "A connection to the server could not be made.";
                    _successfulLogOn = false;
                }

                IsLoading = false;
                return;
            }

            if (!result.IsSuccessful)
            {
                if (result.Errors.Length > 0)
                    Error = result.Errors[0];

                _successfulLogOn = false;
            }
            else
            {
                _userId = result.UserId;
                _successfulLogOn = true;

                var settings = _settingsManager.LoadSettings<LogOnSettings>(EmulationContext.Empty);
                settings.Username = UserName;
                settings.Password = Password;
                settings.UserId = result.UserId;
                _settingsManager.ApplySettings(settings);
            }

            IsLoading = false;
        }

        void Register_DoWork(object sender, DoWorkEventArgs e)
        {
            var curName = UserName;
            var curPassword = Password;
            var curEmail = Email;

            var svc = new AccountService();

            var result = svc.IsRegistrationValid(UserName, Password, Email);

            if (result == null)
            {
                _successfulLogOn = false;

                Error = "A connection to the server could not be made.";
                IsLoading = false;
                return;
            }

            if (!result.IsValid)
            {
                if (result.Errors.Length > 0)
                    Error = result.Errors[0];

                _successfulLogOn = false;
            }
            else
            {
                svc.RegisterUser(result.UserId, curName, curPassword, curEmail);
                Error = String.Empty;
                _userId = result.UserId;
                _successfulLogOn = true;

                var settings = _settingsManager.LoadSettings<LogOnSettings>(EmulationContext.Empty);
                settings.Username = UserName;
                settings.Password = Password;
                _settingsManager.ApplySettings(settings);
            }

            IsLoading = false;
        }

        protected void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
