﻿using Knx.Falcon.Configuration;

namespace KNX_PROJET_2
{
    public class InterfaceViewModel
    {
        public ConnectorType ConnectorType { get; }
        public string DisplayName { get; set; } // Ajout du set pour DisplayName
        public string ConnectionString { get; set; } // Ajout du set pour ConnectionString

        public InterfaceViewModel(ConnectorType connectorType, string displayName, string connectionString)
        {
            ConnectorType = connectorType;
            DisplayName = displayName;
            ConnectionString = connectionString;
        }

        public override string ToString() => DisplayName;
    }

}
