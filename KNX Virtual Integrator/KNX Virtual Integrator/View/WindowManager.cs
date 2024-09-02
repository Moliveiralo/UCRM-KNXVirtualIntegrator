using System.Windows;
using KNX_Virtual_Integrator.View.Windows;
using KNX_Virtual_Integrator.ViewModel;

namespace KNX_Virtual_Integrator.View;

public class WindowManager
{
    private readonly MainViewModel _mainViewModel;

    public WindowManager(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        // Initialisation des fenêtres
        MainWindow = new MainWindow(_mainViewModel);
        //SettingsWindow = new SettingsWindow(_mainViewModel);
        ConnectionWindow = new ConnectionWindow(_mainViewModel);
        TestAnalWindow = new TESTANALYSE(_mainViewModel);
    }

    public MainWindow MainWindow { get; }
    public SettingsWindow? SettingsWindow { get; }
    public ConnectionWindow? ConnectionWindow { get; private set; }
    public TESTANALYSE? TestAnalWindow { get; private set; }

    public void ShowMainWindow() => MainWindow.Show();
    public void ShowSettingsWindow() => SettingsWindow?.Show();
    public void ShowAnalWindow()
    {
        try
        {
            if (TestAnalWindow is not { IsVisible: true })
            {
                TestAnalWindow = new TESTANALYSE(_mainViewModel);
            }

            TestAnalWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'ouverture de la fenêtre : {ex.Message}");
        }
    }

    public void ShowConnectionWindow()
    {
        try
        {
            if (ConnectionWindow is not { IsVisible: true })
            {
                ConnectionWindow = new ConnectionWindow(_mainViewModel);
            }

            ConnectionWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'ouverture de la fenêtre : {ex.Message}");
        }
    }}