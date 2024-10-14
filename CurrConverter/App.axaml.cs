using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CurrConverter.ViewModels;
using CurrConverter.Views;

namespace CurrConverter;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel(["USD", "UAH", "EUR", "GBP", "JPY", "AUD",  "CAD", "CHF", "CNY", "SEK", $"NZD"]),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}