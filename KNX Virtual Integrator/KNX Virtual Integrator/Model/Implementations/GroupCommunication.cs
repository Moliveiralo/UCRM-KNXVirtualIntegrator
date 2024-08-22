using System.Windows;
using GalaSoft.MvvmLight;
using KNX_Virtual_Integrator.Model.Interfaces;
using Knx.Falcon;


namespace KNX_Virtual_Integrator.Model.Implementations;

public class GroupCommunication(BusConnection busConnection) : ObservableObject, IGroupCommunication
{
    private GroupAddress _groupAddress = new("0/1/1");
    public GroupAddress GroupAddress
    {
        get => _groupAddress;
        set => Set(() => GroupAddress, ref _groupAddress, value);
    }

    private GroupValue? _groupValue;
    public GroupValue? GroupValue
    {
        get => _groupValue;
        set => Set(() => GroupValue, ref _groupValue, value);
    }
    public async Task GroupValueWriteOnAsync()
    {
            
        try
        {
            if (busConnection is { IsConnected: true, IsBusy: false })
            {
                _groupValue = new GroupValue(true);
                if (busConnection is { CancellationTokenSource: not null, Bus: not null })
                    await busConnection.Bus.WriteGroupValueAsync(
                        _groupAddress, _groupValue, MessagePriority.High,
                        busConnection.CancellationTokenSource.Token
                    );
            }
            else
            {
                MessageBox.Show("Le bus KNX n'est pas connecté. Veuillez vous connecter d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
                    
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du test de l'envoi de la trame : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                
        }
            
    }
    
    public async Task GroupValueWriteOffAsync()
    {

        try
        {
            if (busConnection is { IsConnected: true, IsBusy: false })
            {
                _groupValue = new GroupValue(false);
                if (busConnection is { CancellationTokenSource: not null, Bus: not null })
                    await busConnection.Bus.WriteGroupValueAsync(
                        _groupAddress, _groupValue, MessagePriority.High,
                        busConnection.CancellationTokenSource.Token
                    );
            }
            else
            {
                MessageBox.Show("Le bus KNX n'est pas connecté. Veuillez vous connecter d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du test de l'envoi de la trame : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                
        }

    }
}