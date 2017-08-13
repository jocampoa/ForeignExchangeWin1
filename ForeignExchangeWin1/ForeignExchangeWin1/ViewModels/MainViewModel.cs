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
        bool _isEnabled;
        bool _isRunning;
        int _sourceRateID;
        int _targetRateID;
        string _amount;
        #endregion

        #region "Properties"
        public ObservableCollection<Rate> Rates { get; set; }

        public int SourceRateID { get; set; }

        public int TargetRateID { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsRunning { get; set; }

        public string Result { get; set; }
        public string Pesos
        {
            get;
            set;
        }

        public string Amount
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
            IsEnabled = false;
            var response = await apiServices.GetRates();
            IsRunning = false;
            IsEnabled = true;
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
            #endregion           
        }

        void ConverterPlus()
        {
            throw new NotImplementedException();
        }
    }
}
