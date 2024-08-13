using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Knx.Falcon;
using Knx.Falcon.KnxnetIp;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Knx.Falcon.Configuration;
using Knx.Falcon.Sdk;

namespace KNX_PROJET_2
{
    public class MainViewModel : ViewModelBase
    {
        private static XNamespace _globalKnxNamespace = "http://knx.org/xml/ga-export/01";

        private KnxBus _bus;
        private CancellationTokenSource _cancellationTokenSource;

        // Propriétés liées à l'interface utilisateur
        public ObservableCollection<string> GroupAddresses { get; private set; }
        public ObservableCollection<InterfaceViewModel> DiscoveredInterfaces { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand RefreshInterfacesCommand { get; private set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set => Set(ref _isBusy, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set => Set(ref _isConnected, value);
        }

        private string _connectionState;
        public string ConnectionState
        {
            get => _connectionState;
            private set => Set(ref _connectionState, value);
        }

        public MainViewModel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            GroupAddresses = new ObservableCollection<string>();
            DiscoveredInterfaces = new ObservableCollection<InterfaceViewModel>();

            ImportCommand = new RelayCommand(async () => await ImportListGroupAddress());
            ConnectCommand = new RelayCommand(async () => await ConnectBusAsync());
            DisconnectCommand = new RelayCommand(async () => await DisconnectBusAsync());
            RefreshInterfacesCommand = new RelayCommand(async () => await DiscoverInterfacesAsync());
        }

        private async Task ImportListGroupAddress()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Sélectionner un fichier XML",
                Filter = "Fichiers XML|*.xml|Tous les fichiers|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    XDocument doc = XDocument.Load(filePath);
                    var allGroupAddresses = doc.Descendants(_globalKnxNamespace + "GroupAddress").ToList();

                    GroupAddresses.Clear();
                    foreach (var groupAddress in allGroupAddresses)
                    {
                        var msg = new StringBuilder();
                        msg.AppendLine("--------------------------------------------------------------------");
                        msg.AppendLine($"Name: {groupAddress.Attribute("Name")?.Value}");
                        msg.AppendLine($"Adresse: {groupAddress.Attribute("Address")?.Value}");

                        GroupAddresses.Add(msg.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors du chargement du fichier XML : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task ConnectBusAsync()
        {
            if (IsBusy)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                IsBusy = true;
                var connectionString = SelectedInterface?.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("Le type de connexion et la chaîne de connexion doivent être fournis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_bus != null)
                {
                    _bus.ConnectionStateChanged -= BusConnectionStateChanged;
                    await _bus.DisposeAsync();
                    _bus = null;
                    UpdateConnectionState();
                }

                var connectorParameters = ConnectorParameters.FromConnectionString(connectionString);

                _bus = new KnxBus(connectorParameters);
                await _bus.ConnectAsync(_cancellationTokenSource.Token);

                if (_bus.ConnectionState == BusConnectionState.Connected)
                {
                    _bus.ConnectionStateChanged += BusConnectionStateChanged;
                    IsConnected = true;
                    UpdateConnectionState();
                    MessageBox.Show("Connexion réussie au bus.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new InvalidOperationException("La connexion au bus a échoué.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion au bus : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task DisconnectBusAsync()
        {
            if (IsBusy || !IsConnected)
            {
                MessageBox.Show("Le bus est déjà déconnecté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;

            try
            {
                if (_bus != null)
                {
                    _bus.ConnectionStateChanged -= BusConnectionStateChanged;
                    await _bus.DisposeAsync();
                    _bus = null;
                    IsConnected = false;
                    UpdateConnectionState();
                    MessageBox.Show("Déconnexion réussie du bus.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Le bus est déjà déconnecté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la déconnexion du bus : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateConnectionState()
        {
            ConnectionState = IsConnected ? "Connected" : "Disconnected";
        }

        private void BusConnectionStateChanged(object sender, EventArgs e)
        {
            UpdateConnectionState();
        }

        private async Task DiscoverInterfacesAsync()
        {
            try
            {
                DiscoveredInterfaces.Clear();

                var ipDiscoveryTask = Task.Run(async () =>
                {
                    var results = KnxBus.DiscoverIpDevicesAsync(CancellationToken.None);
                    await foreach (var result in results)
                    {
                        foreach (var tunnelingServer in result.GetTunnelingConnections())
                        {
                            DiscoveredInterfaces.Add(new InterfaceViewModel(
                                ConnectorType.IpTunneling,
                                tunnelingServer.Name,
                                tunnelingServer.ToConnectionString()));
                        }

                        if (result.Supports(ServiceFamily.Routing, 1))
                        {
                            var routingParameters = IpRoutingConnectorParameters.FromDiscovery(result);
                            DiscoveredInterfaces.Add(new InterfaceViewModel(
                                ConnectorType.IpRouting,
                                $"{result.MulticastAddress} on {result.LocalIPAddress}",
                                routingParameters.ToConnectionString()));
                        }
                    }
                });

                var usbDiscoveryTask = Task.Run(() =>
                {
                    foreach (var usbDevice in KnxBus.GetAttachedUsbDevices())
                    {
                        DiscoveredInterfaces.Add(new InterfaceViewModel(
                            ConnectorType.Usb,
                            usbDevice.DisplayName,
                            usbDevice.ToConnectionString()));
                    }
                });

                await Task.WhenAll(ipDiscoveryTask, usbDiscoveryTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la découverte des interfaces : {ex.Message}");
            }
        }

        private InterfaceViewModel _selectedInterface;
        public InterfaceViewModel SelectedInterface
        {
            get => _selectedInterface;
            set => Set(ref _selectedInterface, value);
        }
    }
}
