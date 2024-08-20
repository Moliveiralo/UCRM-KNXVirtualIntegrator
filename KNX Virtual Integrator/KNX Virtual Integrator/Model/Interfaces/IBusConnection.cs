namespace KNX_Virtual_Integrator.Model.Interfaces;

public interface IBusConnection
{ 
    Task ConnectBusAsync();

    Task DisconnectBusAsync();

    Task DiscoverInterfacesAsync();
}