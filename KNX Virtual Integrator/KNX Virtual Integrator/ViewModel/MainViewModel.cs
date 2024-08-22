﻿using System.Windows;
using System.Windows.Input;
using KNX_Virtual_Integrator.Model;
using KNX_Virtual_Integrator.Model.Interfaces;
using KNX_Virtual_Integrator.ViewModel.Commands;
using ICommand = KNX_Virtual_Integrator.ViewModel.Commands.ICommand;

// ReSharper disable InvalidXmlDocComment
// ReSharper disable NullableWarningSuppressionIsUsed

namespace KNX_Virtual_Integrator.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    /* ------------------------------------------------------------------------------------------------
    ------------------------------------------- ATTRIBUTS  --------------------------------------------
    ------------------------------------------------------------------------------------------------ */
    public string ProjectFolderPath { get; private set; } // Stocke le chemin du dossier projet
    
    public IApplicationSettings AppSettings => _modelManager.AppSettings;
    private readonly ModelManager _modelManager;  // Référence à ModelManager

    

    
    /* ------------------------------------------------------------------------------------------------
    ----------------------------------------- CONSTRUCTEUR  -------------------------------------------
    ------------------------------------------------------------------------------------------------ */
    public MainViewModel(ModelManager modelManager)
    {
        // Initialisation des attributs
        ProjectFolderPath = "";
        
        
        // Initialisation du modelmanager
        _modelManager = modelManager;
        
        
        // Initialisation des commandes relais sans valeur de retour
        ConsoleAndLogWriteLineCommand = new RelayCommand<string>(
            parameter =>
            {
                if (!string.IsNullOrWhiteSpace(parameter)) modelManager.Logger.ConsoleAndLogWriteLine(parameter);
            }
        );
        
        ExtractGroupAddressCommand = new RelayCommand<object>(_ => modelManager.GroupAddressManager.ExtractGroupAddress());
        
        CreateDebugArchiveCommand = new RelayCommand<(bool IncludeOsInfo, bool IncludeHardwareInfo, bool IncludeImportedProjects)>(
            parameters =>
            {
                modelManager.DebugArchiveGenerator.CreateDebugArchive(parameters.IncludeOsInfo,
                    parameters.IncludeHardwareInfo,
                    parameters.IncludeImportedProjects);
            }
        );
        
        FindZeroXmlCommand = new RelayCommand<string>(fileName => modelManager.FileFinder.FindZeroXml(fileName));
        
        ConnectBusCommand = new RelayCommand<object>(_ => modelManager.BusConnection.ConnectBusAsync());
        
        DisconnectBusCommand = new RelayCommand<object>(_ => modelManager.BusConnection.DisconnectBusAsync());
        
        RefreshInterfacesCommand = new RelayCommand<object>(_ => modelManager.BusConnection.DiscoverInterfacesAsync());
        
        GroupValueWriteOnCommand = new RelayCommand<object>(_ => modelManager.GroupCommunication.GroupValueWriteOnAsync());
        
        GroupValueWriteOffCommand = new RelayCommand<object>(_ => modelManager.GroupCommunication.GroupValueWriteOffAsync());
        
        SaveSettingsCommand = new RelayCommand<object>(_ => modelManager.AppSettings.Save());
        
        
        
        ExtractGroupAddressFileCommand = new RelayCommandWithResult<string, bool>(fileName => 
            modelManager.ProjectFileManager.ExtractGroupAddressFile(fileName));
        
        ExtractProjectFilesCommand = new RelayCommandWithResult<string, bool>(fileName =>
        {
            var success = _modelManager.ProjectFileManager.ExtractProjectFiles(fileName);
            ProjectFolderPath = _modelManager.ProjectFileManager.ProjectFolderPath;
            return success;
        });
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
    public ICommand ConsoleAndLogWriteLineCommand { get; }

    
    /// <summary>
    /// Command that extracts a group address using the GroupAddressManager.
    /// </summary>
    public ICommand ExtractGroupAddressCommand { get; }

    
    /// <summary>
    /// Command that creates a debug archive with optional OS info, hardware info, and imported projects.
    /// </summary>
    /// <param name="IncludeOsInfo">Specifies whether to include OS information in the debug archive.</param>
    /// <param name="IncludeHardwareInfo">Specifies whether to include hardware information in the debug archive.</param>
    /// <param name="IncludeImportedProjects">Specifies whether to include imported projects in the debug archive.</param>
    public ICommand CreateDebugArchiveCommand { get; }

    
    /// <summary>
    /// Command that finds a zero XML file based on the provided file name.
    /// </summary>
    /// <param name="fileName">The name of the file to find.</param>
    public ICommand FindZeroXmlCommand { get; }

    
    /// <summary>
    /// Command that connects to the bus asynchronously.
    /// </summary>
    public ICommand ConnectBusCommand { get; }

    
    /// <summary>
    /// Command that disconnects from the bus asynchronously.
    /// </summary>
    public ICommand DisconnectBusCommand { get; }

    
    /// <summary>
    /// Command that refreshes the list of bus interfaces asynchronously.
    /// </summary>
    public ICommand RefreshInterfacesCommand { get; }

    
    /// <summary>
    /// Command that sends a group value write "on" command asynchronously.
    /// </summary>
    public ICommand GroupValueWriteOnCommand { get; }

    
    /// <summary>
    /// Command that sends a group value write "off" command asynchronously.
    /// </summary>
    public ICommand GroupValueWriteOffCommand { get; }

    
    /// <summary>
    /// Command that saves the current application settings.
    /// </summary>
    public ICommand SaveSettingsCommand { get; }

    
    
    
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
    public ICommand ExtractGroupAddressFileCommand { get; }

    
    /// <summary>
    /// Command that extracts project files based on the provided file name and returns a boolean indicating success.
    /// </summary>
    /// <param name="fileName">The name of the file to extract.</param>
    /// <returns>True if the extraction was successful; otherwise, false.</returns>
    public ICommand ExtractProjectFilesCommand { get; }


    
    
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