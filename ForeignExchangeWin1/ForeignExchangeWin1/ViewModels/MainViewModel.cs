namespace ForeignExchangeWin1.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using System;
    using Xamarin.Forms;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using Models;
    using Services;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region "Events"
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region "Services"
        ApiService apiServices;
        #endregion

        #region "Attributes"
        string _dollars;
        string _euros;
        string _pounds;
        ObservableCollection<Rate> _rates;
        string _result;
        bool _isEnabled;
        bool _isRunning;
        int _sourceRateID;
        int _targetRateID;
        string _amount;
        #endregion

        #region "Properties"
        public string Amount { get; set; }
        public ObservableCollection<Rate> Rates
        {
            get
            {
                return _rates;
            }
            set
            {
                if (_rates != value)
                {
                    _rates = value;
                    PropertyChanged?.Invoke(this,
                        new PropertyChangedEventArgs(nameof(Rates)));
                }
            }
        }

        public Rate SourceRateID { get; set; }

        public Rate TargetRateID { get; set; }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this,
                        new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    PropertyChanged?.Invoke(this,
                        new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }
        public string Pesos
        {
            get;
            set;
        }

        public string Dollars
        {
            get { return _dollars; }
            set
            {
                if (value != _dollars)
                {
                    _dollars = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(Dollars)));
                }
            }
        }

        public string Euros
        {
            get { return _euros; }
            set
            {
                if (value != _euros)
                {
                    _euros = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(Euros)));
                }
            }
        }

        public string Pounds
        {
            get { return _pounds; }
            set
            {
                if (value != _pounds)
                {
                    _pounds = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(Pounds)));
                }
            }
        }
        #endregion

        #region "Constructors"
        public MainViewModel()
        {
            apiServices = new ApiService();
            Result = "Enter an amount, select source rate, select target rate and press convert button";
            LoadRates();
        }
        #endregion

        #region "Methods"
        async void LoadRates()
        {
            IsRunning = true;
            Result = "Loading rates...";
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://apiexchangerates.azurewebsites.net");
                var controller = "/api/Rates";
                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    IsRunning = false;
                    Result = result;
                }

                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);
                Rates = new ObservableCollection<Rate>(rates);
                IsRunning = false;
                IsEnabled = true;
                Result = "Ready to convert!";
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Result = ex.Message;              
            }

            IsEnabled = false;
           

        }
        #endregion

        #region "Commands"
        public ICommand ConvertCommand
        {
            get { return new RelayCommand(Convert); }
        }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Pesos))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must enter a value in pesos...", "Accept");
                return;
            }

            decimal pesos = 0;
            if (!decimal.TryParse(Pesos, out pesos))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must enter a value numeric in pesos.", "Accept");
                Pesos = null;
                return;
            }

            var dollars = pesos / 3003.003M;
            var euros = pesos / 3531.05105M;
            var pounds = pesos / 3907.23724M;

            Dollars = string.Format("${0:N2}", dollars);
            Euros = string.Format("€{0:N2}", euros);
            Pounds = string.Format("£{0:N2}", pounds);
        }

        public ICommand ConverterPlusCommand
        {
            get
            {
                return new RelayCommand(ConverterPlus);
            }
        }
        async void ConverterPlus()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a value in amount.",
                    "Accept");
                return;
            }

            decimal amount = 0;
            if(!decimal.TryParse(Amount, out amount))
            {
                await Application.Current.MainPage.DisplayAlert(
                "Error",
                "You must enter a numeric value in amount.",
                "Accept");
                return;
            }

            if (SourceRateID == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must select a source rate",
                    "Accept");
                return;
            }

            if (TargetRateID == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must select a target rate",
                    "Accept");
                return;
            }

            var amountConverted = amount / (decimal)SourceRateID.TaxRate * (decimal)TargetRateID.TaxRate;
            Result = string.Format("{0} {1:C2} = {2} {3:C2}",
                SourceRateID.Code, amount, TargetRateID.Code, amountConverted);
        }
        #endregion
    }
}

