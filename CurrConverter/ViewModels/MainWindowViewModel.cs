using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace CurrConverter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        
        private string? _result; // Make _result nullable
        public string? Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private RelayCommand? _convertCommand; // Make _convertCommand nullable
        public RelayCommand ConvertCommand => _convertCommand ??= new RelayCommand(ConvertCurrencyAsync); // Remove async lambda

        private async void ConvertCurrencyAsync() // Change return type to void
        {
            var fromCurrency = "USD";
            var toCurrency = "UAH";
            var amount = 1.0;

            try
            {
                var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";
                var apiResponse = await GetApiResponseAsync(url);

                if (apiResponse?.Rates != null && apiResponse.Rates.TryGetValue(toCurrency, out var rate))
                {
                    var convertedAmount = amount * rate;
                    Result = $"{amount} {fromCurrency} = {convertedAmount:F2} {toCurrency} (Rate: {rate})";
                }
                else
                {
                    throw new Exception($"Неизвестная пара валют: {fromCurrency} в {toCurrency}.");
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
        }

        private static async Task<ExchangeRateApiResponse?> GetApiResponseAsync(string url)
        {
            var jsonResponse = await HttpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<ExchangeRateApiResponse>(jsonResponse);
        }

        public class ExchangeRateApiResponse
        {
            public Dictionary<string, double>? Rates { get; set; } // Make Rates nullable
        }
    }
}