using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace CurrConverter.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        private static readonly HttpClient HttpClient = new();
    
        public string? Result {
            get => _result;
            set {
                _result = value;
                OnPropertyChanged();
            }
        }
        private string? _result;
        public RelayCommand ConvertCommand { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public double? Amount { get; set; }
        public ObservableCollection<string> AvailableCurrencies { get; set; }
        public bool IsCurrenciesLoaded { get; set; }

        public MainWindowViewModel() {
            AvailableCurrencies = [];
            IsCurrenciesLoaded = false;
            FetchAvailableCurrenciesAsync();
            ConvertCommand = new RelayCommand(ConvertCurrencyAsync);
        }
    
        private async Task FetchAvailableCurrenciesAsync() {
            // Fetch currencies
            IsCurrenciesLoaded = false;
            var apiResponse = await GetApiResponseAsync("https://api.exchangerate-api.com/v4/latest/USD");
            if (apiResponse?.Rates != null)
            {
                AvailableCurrencies.Clear();
                foreach (var currency in apiResponse.Rates.Keys)
                {
                    AvailableCurrencies.Add(currency);
                }
                IsCurrenciesLoaded = true;
            }
        }
        
        private async void ConvertCurrencyAsync() {   
            if (FromCurrency == null || ToCurrency == null || Amount == null) {
                Result = "Fault input";
                return;
            }

            try {
                if (!double.TryParse(Amount.ToString(), out double amountValue)) {
                    throw new InvalidCastException("Invalid format");
                }

                var apiResponse = await GetApiResponseAsync($"https://api.exchangerate-api.com/v4/latest/{FromCurrency}");

                if (apiResponse?.Rates != null && apiResponse.Rates.TryGetValue(ToCurrency, out var rate)) {
                    Result = $"{amountValue * rate:F3} {ToCurrency}";
                }
                else { throw new Exception("Failed to retrieve"); }
            } catch (InvalidCastException ex) {
                Result = ex.Message;
            } catch (Exception ex) {
                Result = ex.Message; 
            }
        }

        
        public class ExchangeRateApiResponse { public Dictionary<string, double>? Rates { get; set; } }
        
        private static async Task<ExchangeRateApiResponse?> GetApiResponseAsync(string url) { 
            return JsonConvert.DeserializeObject<ExchangeRateApiResponse>(await HttpClient.GetStringAsync(url));
        }
    }
}