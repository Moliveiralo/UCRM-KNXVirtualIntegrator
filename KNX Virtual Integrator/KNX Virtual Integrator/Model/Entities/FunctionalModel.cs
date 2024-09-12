using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Knx.Falcon.ApplicationData.MasterData;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Entities;
/// <summary>
/// Represents a functional model with a unique key, name, and DPT (Data Point Type) value.
///
/// This class provides properties for storing and retrieving the key and name of the model.
/// It includes functionality for displaying the model in a formatted string and implements 
/// equality comparison based on the DPT value, name, and key. The class is designed to be 
/// comparable with other instances of the same type to determine equality.
///
/// - Key: Unique identifier for the functional model.
/// - Name: Descriptive name of the functional model.
/// 
/// The class overrides the ToString, Equals, and GetHashCode methods to provide custom
/// string representation, equality checks, and hash code generation.
/// </summary>

public class FunctionalModel : IEquatable<FunctionalModel>
{
    //Attributs
    //private int dpt_value; // Les DPT sont d'un type plus complexe, "int" est temporaire
    public DatapointType DptValue { get; set; }

    public int Key { get; set; } // Identifiant unique des modèles, utilisé notement sur l'interface "M{Key}"
    public string Name { get; set; } //Nom donné par l'utilisateur, Modifiable

    // Nouvelles propriétés pour modele par defaut
    public DatapointSubtype SubDPT { get; set; }      // Sous-type du DPT
    public int SizeInBit { get; set; }   // Taille en bits
    public GroupValue[] Ve { get; set; } // Tableau pour valeurs d'entrée
    public GroupValue[] Va { get; set; } // Tableau pour valeurs attendues

    //Constructeur
    public FunctionalModel(DatapointType value, string name, DatapointSubtype subDpt, int sizeInBit, GroupValue[] ve, GroupValue[] va) //attention a changer le type
    {
        //dpt_value = value;
        DptValue = value;
        Name = name;
        SubDPT = subDpt;
        SizeInBit = sizeInBit;
        Ve = ve;
        Va = va;
    }

    //Methodes
    public override string ToString() //Fonction utilisé dès que l'affchage d'un modèle et demandé, utilisé pas la vue.
    {
        return $"M{Key} | {Name}";
    }

    public override bool Equals(object? obj) //Non utilisé mais verifie l'unicité d'un modèle, venir modifier en cas d'ajout d'attributs
    {
        if (obj is FunctionalModel other)
        {
            return Equals(other);
        }
        return false;
    }

    public bool Equals(FunctionalModel? other)
    {
        if (other is null) return false;
        //return dpt_value == other.dpt_value && Name == other.Name && Key == other.Key;
        return DptValue == other.DptValue && Name == other.Name && Key == other.Key;
    }

    public override int GetHashCode()
    {
        //return HashCode.Combine(dpt_value, Name, Key);
        return HashCode.Combine(DptValue, Name, Key);
    }

}