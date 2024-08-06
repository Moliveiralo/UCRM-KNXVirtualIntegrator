﻿namespace KNX_Virtual_Integrator;

public class DisplayElements
{
    /* ------------------------------------------------------------------------------------------------
    ------------------------------------------- ATTRIBUTS  --------------------------------------------
    ------------------------------------------------------------------------------------------------ */
    /// <summary>
    /// Represents the main window instance of the application.
    /// </summary>
    public MainWindow MainWindow { get; } = new();
    
    /* ------------------------------------------------------------------------------------------------
   -------------------------------------------- METHODES  --------------------------------------------
   ------------------------------------------------------------------------------------------------ */
    /// <summary>
    /// Shows the main window of the application.
    /// </summary>
    public void ShowMainWindow()
    {
        MainWindow.Show();
    }
}