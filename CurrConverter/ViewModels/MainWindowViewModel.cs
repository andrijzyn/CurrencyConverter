using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace CurrConverter.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        private static readonly HttpClient HttpClient = new();

        public string? Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        private string? _result;
        public RelayCommand ConvertCommand { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public double? Amount { get; set; }
        public List<string> AvailableCurrencies { get; set; }

        public MainWindowViewModel(List<string> availableCurrencies)
        {
            AvailableCurrencies = availableCurrencies;
            ConvertCommand = new RelayCommand(ConvertCurrencyAsync);
        }

        private async void ConvertCurrencyAsync()
        {
            if (FromCurrency == null || ToCurrency == null || Amount == null)
            {
                Result = "Please select currencies and input amount";
                return;
            }

            try
            {
                var apiResponse = await GetApiResponseAsync($"https://api.exchangerate-api.com/v4/latest/{FromCurrency}");

                if (apiResponse?.Rates != null && apiResponse.Rates.TryGetValue(ToCurrency, out var rate)) {
                    Result = $"{Amount:F2} {FromCurrency} = {Amount.Value * rate:F2} {ToCurrency} (Rate: {rate})";
                }else { throw new Exception($"Unknown currency pair: {FromCurrency} to {ToCurrency}."); }
            } catch (Exception ex) { Result = ex.Message; }
        }

        private static async Task<ExchangeRateApiResponse?> GetApiResponseAsync(string url) { 
            return JsonConvert.DeserializeObject<ExchangeRateApiResponse>(await HttpClient.GetStringAsync(url));
        }

        public class ExchangeRateApiResponse { public Dictionary<string, double>? Rates { get; set; } }
    }
}