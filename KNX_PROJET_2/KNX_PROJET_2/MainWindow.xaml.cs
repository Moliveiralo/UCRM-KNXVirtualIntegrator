﻿using System.Windows;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Interop;
using System.IO;
using Knx.Falcon.Sdk;
using System.Windows.Controls;
using Knx.Falcon.Configuration;
using Knx.Falcon;

namespace KNX_PROJET_2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //ATTRIBUTS

        /// <summary>
        /// Represents the global XML namespace for KNX projects.
        /// </summary>
        private static XNamespace _globalKnxNamespace = "http://knx.org/xml/ga-export/01";

        /// <summary>
        /// Gets the path to the exported project folder.
        /// </summary>
        public string ProjectFolderPath { get; private set; } = "";

        



        // Gestion du clic sur le bouton Importer
        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            // Créer une instance de OpenFileDialog pour sélectionner un fichier XML
            OpenFileDialog openFileDialog = new()
            {
                Title = "Sélectionner un fichier XML",
                Filter = "Fichiers XML|*.xml|Tous les fichiers|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            // Afficher la boîte de dialogue et vérifier la sélection
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Appeler la méthode asynchrone pour afficher les adresses de groupe
                await ImportListGroupAddress(filePath);
            }
        }
        
        


        
        //TACHE IMPORTER LISTE DES ADRESSES DE GROUPE
        private async Task ImportListGroupAddress(string filePath)
        {
            try
            {
                // Charger les adresses de groupe à partir du fichier XML
                XDocument doc = XDocument.Load(filePath);

                // Supposez que les adresses de groupe sont stockées sous une balise <GroupAddress> dans le XML
                var allGroupAddresses = doc.Descendants(_globalKnxNamespace + "GroupAddress").ToList();

                var addresses = new List<string>();

                foreach (var groupAddress in allGroupAddresses)
                {
                    var msg = new StringBuilder();
                    msg.AppendLine("--------------------------------------------------------------------");
                    msg.AppendLine($"Name: {groupAddress.Attribute("Name")?.Value}");
                    msg.AppendLine($"Adresse: {groupAddress.Attribute("Address")?.Value}");

                    // Ajouter les adresses au message
                    addresses.Add(msg.ToString());

                    // Mettre à jour le contrôle UI avec les adresses
                    if (GroupAddressListControl != null)
                    {
                        GroupAddressListControl.UpdateGroupAddresses(addresses);
                    }
                    else
                    {
                        MessageBox.Show("Le contrôle de la liste des adresses de groupe n'est pas initialisé correctement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fichier XML : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }












        private KnxBus _bus;
        private CancellationTokenSource _cancellationTokenSource;

        //Gestion du clic sur le bouton Connect
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            await ConnectBusAsync();
        }

        //Gestion du clic sur le bouton Disconnect
        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await DisconnectBusAsync();
        }



        //TACHE POUR CONNECTION AU BUS
        private async Task ConnectBusAsync()
        {
            if (IsBusy)
                return;

            //_cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Récupérer le type de connexion sélectionné
                string connectionString = ((ComboBoxItem)ConnectionTypeComboBox.SelectedItem)?.Content.ToString();
                

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("Le type de connexion et la chaîne de connexion doivent être fournis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Vérifier si un bus est déjà connecté, et le déconnecter si nécessaire
                if (_bus != null)
                {
                    _bus.ConnectionStateChanged -= BusConnectionStateChanged;
                    await _bus.DisposeAsync();
                    _bus = null;
                    UpdateConnectionState();
                }

                var connectorParameters = CreateConnectorParameters(connectionString);

                // Connexion au bus
                var bus = new KnxBus(connectorParameters);
                await bus.ConnectAsync(_cancellationTokenSource.Token);
                _bus = bus;

                _bus.ConnectionStateChanged += BusConnectionStateChanged;
                UpdateConnectionState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion au bus : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //TACHE POUR DECONNECTION AU BUS

        private async Task DisconnectBusAsync()
        {
            if (IsBusy || !IsConnected)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            try
            {
                if (_bus != null)
                {
                    _bus.ConnectionStateChanged -= BusConnectionStateChanged;
                    await _bus.DisposeAsync();
                    _bus = null;
                }

                UpdateConnectionState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la déconnexion du bus : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateConnectionState()
        {
            // Cette méthode doit mettre à jour l'état de la connexion dans l'interface utilisateur
            // Par exemple, vous pouvez mettre à jour des propriétés liées à la connexion
            // Exemple :
            // ConnectionStateTextBlock.Text = IsConnected ? "Connected" : "Disconnected";
            // ou notifier d'autres parties de l'interface utilisateur
        }

        private void BusConnectionStateChanged(object sender, EventArgs e)
        {
            // Cette méthode sera appelée lorsque l'état de la connexion change
            // Vous pouvez mettre à jour l'interface utilisateur en conséquence
            UpdateConnectionState();
        }

        private ConnectorParameters CreateConnectorParameters(string connectionString)
        {
            // Créez les paramètres du connecteur en fonction du type sélectionné
            switch (connectionString)
            {
                case "USB":
                    // Créer les paramètres pour la connexion USB
                    return ConnectorParameters.FromConnectionString(connectionString); // Assurez-vous que la chaîne de connexion est correcte pour USB
                case "IP":
                    // Créer les paramètres pour la connexion IP
                    return ConnectorParameters.FromConnectionString(connectionString); // Assurez-vous que la chaîne de connexion est correcte pour IP
                default:
                    throw new InvalidOperationException("Type de connexion inconnu.");
            }
        }

        public bool IsBusy => _cancellationTokenSource?.Token.IsCancellationRequested == false;

        public bool IsConnected => _bus != null && _bus.ConnectionState == BusConnectionState.Connected;

    }


}



