﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using KNX_Virtual_Integrator.Model;
using KNX_Virtual_Integrator.Model.Interfaces;
using KNX_Virtual_Integrator.View;
using KNX_Virtual_Integrator.ViewModel.Commands;
using ICommand = KNX_Virtual_Integrator.ViewModel.Commands.ICommand;
using System.ComponentModel;


namespace KNX_Virtual_Integrator.ViewModel
{
    public partial class MainViewModel
    {
        private GridLength _modelColumnWidth = new GridLength(1, GridUnitType.Auto);
        private GridLength _adressColumnWidth = new GridLength(1, GridUnitType.Auto);

        public GridLength ModelColumnWidth
        {
            get { return _modelColumnWidth; }
            set
            {
                if (_modelColumnWidth != value)
                {
                    _modelColumnWidth = value;
                    OnPropertyChanged(nameof(ModelColumnWidth)); // Notification du changement
                }
            }
        }
        public GridLength AdressColumnWidth
        {
            get { return _adressColumnWidth; }
            set
            {
                if (_adressColumnWidth != value)
                {
                    _adressColumnWidth = value;
                    OnPropertyChanged(nameof(AdressColumnWidth)); // Notification du changement
                }
            }
        }

        public RelayCommand HideModelColumnCommand { get; private set; }
        public RelayCommand HideAdressColumnCommand { get; private set; }
        public RelayCommand ShowModelColumnCommand { get; private set; }
        public RelayCommand ShowAdressColumnCommand { get; private set; }

        private void HideModelColumn()
        {
            ModelColumnWidth = new GridLength(0);
        }

        private void HideAdressColumn()
        {
            AdressColumnWidth = new GridLength(0);
        }
        private void ShowModelColumn()
        {
            ModelColumnWidth = GridLength.Auto;
        }

        private void ShowAdressColumn()
        {
            AdressColumnWidth = GridLength.Auto;
        }
    }
}
