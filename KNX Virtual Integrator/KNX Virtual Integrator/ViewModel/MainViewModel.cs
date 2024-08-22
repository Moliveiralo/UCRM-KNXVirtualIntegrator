﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KNX_Virtual_Integrator.Model;
using KNX_Virtual_Integrator.Model.Interfaces;
using KNX_Virtual_Integrator.View;
using KNX_Virtual_Integrator.ViewModel.Commands;
using ICommand = KNX_Virtual_Integrator.ViewModel.Commands.ICommand;

// ReSharper disable InvalidXmlDocComment
// ReSharper disable NullableWarningSuppressionIsUsed

namespace KNX_Virtual_Integrator.ViewModel;

public class MainViewModel : ObservableObject, INotifyPropertyChanged
{
    private readonly ModelManager _modelManager;
    private readonly WindowManager? _windowManager;
    /* ------------------------------------------------------------------------------------------------
    ------------------------------------------- ATTRIBUTS  --------------------------------------------
    ------------------------------------------------------------------------------------------------ */
    public string ProjectFolderPath = "";
    
    private readonly IBusConnection _busConnection;
    
    public IApplicationSettings AppSettings => _modelManager.AppSettings;
    
    public ObservableCollection<ConnectionInterfaceViewModel> DiscoveredInterfaces => _busConnection.DiscoveredInterfaces;
    public string SelectedConnectionType
    {
        get => _busConnection.SelectedConnectionType;
        set
        {
            if (_busConnection.SelectedConnectionType != value)
            {
                _busConnection.SelectedConnectionType = value;
                _busConnection.OnSelectedConnectionTypeChanged();
            }
        }
    }
    public ConnectionInterfaceViewModel? SelectedInterface
    {
        get => _busConnection.SelectedInterface;
        set
        {
            if (_busConnection.SelectedInterface != value)
            {
                _busConnection.SelectedInterface = value;
            }
        }
    }

    
    /* ------------------------------------------------------------------------------------------------
    -------------------------------- COMMANDES SANS VALEUR DE RETOUR  ---------------------------------
    ------------------------------------------------------------------------------------------------ */
    // Patterne d'utilisation :
    // MaCommande.Execute(Args)
    //
    // Si la fonction n'a pas d'arguments, la déclarer en tant que commande dont les paramètres sont de type "object"
    // et lors de l'utilisation, on écrira macommande.Execute(null);
    // Pour un exemple, voir : ExtractGroupAddressCommand
    
    
    
    
    /// <summary>
    /// Command that writes a line of text to the console and log if the provided parameter is not null or whitespace.
    /// </summary>
    public ICommand ConsoleAndLogWriteLineCommand { get; private set; }

    
    /// <summary>
    /// Command that extracts a group address using the GroupAddressManager.
    /// </summary>
    public ICommand ExtractGroupAddressCommand { get; private set; }

    
    /// <summary>
    /// Command that ensures the settings file exists. Creates the file if it does not exist, using the provided file path.
    /// </summary>
    public ICommand EnsureSettingsFileExistsCommand { get; private set; }

    
    /// <summary>
    /// Command that creates a debug archive with optional OS info, hardware info, and imported projects.
    /// </summary>
    /// <param name="IncludeOsInfo">Specifies whether to include OS information in the debug archive.</param>
    /// <param name="IncludeHardwareInfo">Specifies whether to include hardware information in the debug archive.</param>
    /// <param name="IncludeImportedProjects">Specifies whether to include imported projects in the debug archive.</param>
    public ICommand CreateDebugArchiveCommand { get; private set; }

    
    /// <summary>
    /// Command that finds a zero XML file based on the provided file name.
    /// </summary>
    /// <param name="fileName">The name of the file to find.</param>
    public ICommand FindZeroXmlCommand { get; private set; }

    public System.Windows.Input.ICommand OpenConnectionWindowCommand { get; }
    
    /// <summary>
    /// Command that connects to the bus asynchronously.
    /// </summary>
    public System.Windows.Input.ICommand ConnectBusCommand { get; private set; }

    
    /// <summary>
    /// Command that disconnects from the bus asynchronously.
    /// </summary>
    public System.Windows.Input.ICommand DisconnectBusCommand { get; private set; } 

    
    /// <summary>
    /// Command that refreshes the list of bus interfaces asynchronously.
    /// </summary>
    public System.Windows.Input.ICommand RefreshInterfacesCommand { get; private set; }

    
    /// <summary>
    /// Command that sends a group value write "on" command asynchronously.
    /// </summary>
    public System.Windows.Input.ICommand GroupValueWriteOnCommand { get; private set; }

    
    /// <summary>
    /// Command that sends a group value write "off" command asynchronously.
    /// </summary>
    public System.Windows.Input.ICommand GroupValueWriteOffCommand { get; private set; }

    
    /// <summary>
    /// Command that saves the current application settings.
    /// </summary>
    public ICommand SaveSettingsCommand { get; private set; }

    
    
    /* ------------------------------------------------------------------------------------------------
    -------------------------------- COMMANDES AVEC VALEUR DE RETOUR  ---------------------------------
    ------------------------------------------------------------------------------------------------ */
    // Patterne d'utilisation :
    // if (_viewModel.LENOMDELACOMMANDE is RelayCommandWithResult<typeDuParametre, typeDeRetour> command)
    // {
    //      [UTILISATION DE LA COMMANDE]
    // }
    // else
    // {
    //      [GESTION DE L'ERREUR SI LA COMMANDE N'EST PAS DU TYPE ESPERE]
    // }
    //
    //
    // Ou sinon :
    // var maCommande = _viewModel.NOMDELACOMMANDE as RelayCommandWithResult<typeDuParametre, typeDeRetour>;
    //
    // if (maCommande != null) [UTILISATION DE maCommande DIRECTEMENT]
    
    
    
    
    /// <summary>
    /// Command that extracts a group address file based on the provided file name and returns a boolean indicating success.
    /// </summary>
    /// <param name="fileName">The name of the file to extract.</param>
    /// <returns>True if the extraction was successful; otherwise, false.</returns>
    public ICommand ExtractGroupAddressFileCommand { get; private set; }

    
    /// <summary>
    /// Command that extracts project files based on the provided file name and returns a boolean indicating success.
    /// </summary>
    /// <param name="fileName">The name of the file to extract.</param>
    /// <returns>True if the extraction was successful; otherwise, false.</returns>
    public ICommand ExtractProjectFilesCommand { get; private set; }
    
    
    public MainViewModel(ModelManager modelManager)
    {
        _modelManager = modelManager;
        _windowManager = new WindowManager(this);

        _busConnection = _modelManager.BusConnection;
        
        _busConnection.SelectedConnectionType = "Type=USB";
        
        ConsoleAndLogWriteLineCommand = new Commands.RelayCommand<string>(
                parameter =>
                {
                    if (!string.IsNullOrWhiteSpace(parameter)) modelManager.Logger.ConsoleAndLogWriteLine(parameter);
                }
            );
        ExtractGroupAddressCommand = new Commands.RelayCommand<object>(_ => modelManager.GroupAddressManager.ExtractGroupAddress());
        
        EnsureSettingsFileExistsCommand = new Commands.RelayCommand<string>(
            parameter =>
            {
                if (!string.IsNullOrWhiteSpace(parameter)) modelManager.ApplicationFileManager.EnsureSettingsFileExists(parameter);
            }
        );
        
        CreateDebugArchiveCommand = new Commands.RelayCommand<(bool IncludeOsInfo, bool IncludeHardwareInfo, bool IncludeImportedProjects)>(
            parameters =>
            {
                modelManager.DebugArchiveGenerator.CreateDebugArchive(parameters.IncludeOsInfo,
                    parameters.IncludeHardwareInfo,
                    parameters.IncludeImportedProjects);
            }
        );
        
        FindZeroXmlCommand = new Commands.RelayCommand<string>(fileName => modelManager.FileFinder.FindZeroXml(fileName));
        
        OpenConnectionWindowCommand = new RelayCommand(() => _windowManager.ShowConnectionWindow());
        ConnectBusCommand = new RelayCommand(()  => modelManager.BusConnection.ConnectBusAsync());
        DisconnectBusCommand = new RelayCommand(() => modelManager.BusConnection.DisconnectBusAsync());
        RefreshInterfacesCommand = new RelayCommand(() => modelManager.BusConnection.DiscoverInterfacesAsync());
        GroupValueWriteOnCommand = new RelayCommand(() => modelManager.GroupCommunication.GroupValueWriteOnAsync());
        GroupValueWriteOffCommand = new RelayCommand(() => modelManager.GroupCommunication.GroupValueWriteOffAsync());
        
        SaveSettingsCommand = new Commands.RelayCommand<object>(_ => modelManager.AppSettings.Save());
        
        ExtractGroupAddressFileCommand = new RelayCommandWithResult<string, bool>(fileName => 
            modelManager.ProjectFileManager.ExtractGroupAddressFile(fileName));
        ExtractProjectFilesCommand = new RelayCommandWithResult<string, bool>(fileName =>
            modelManager.ProjectFileManager.ExtractProjectFiles(fileName));
    }
    
    
    /* ------------------------------------------------------------------------------------------------
    -------------------------------------------- HANDLERS  --------------------------------------------
    ------------------------------------------------------------------------------------------------ */
    /// <summary>
    /// Handles the event when the left mouse button is pressed down on the slider.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the mouse button event.</param>
    public void SliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _modelManager.SettingsSliderClickHandler.SliderMouseLeftButtonDown(sender, e);
    }

    /// <summary>
    /// Handles the event when the left mouse button is released on the slider.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the mouse button event.</param>
    public void SliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _modelManager.SettingsSliderClickHandler.SliderMouseLeftButtonUp(sender, e);
    }

    /// <summary>
    /// Handles the event when the mouse is moved over the slider while dragging.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the mouse movement event.</param>
    public void SliderMouseMove(object sender, MouseEventArgs e)
    {
        _modelManager.SettingsSliderClickHandler.SliderMouseMove(sender, e);
    }

    public void OnSliderClick(object sender, RoutedEventArgs e)
    {
        _modelManager.SettingsSliderClickHandler.OnSliderClick(sender, e);
    }
    
}