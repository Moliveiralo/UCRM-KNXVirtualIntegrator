using System.ComponentModel;

namespace KNXIntegrator.Models
{
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
        private int dpt_value; // Les DPT sont d'un type plus complexe, "int" est temporaire
        public int Key { get; set; } // Identifiant unique des mod�les, utilis� notement sur l'interface "M{Key}"
        public string Name { get; set; } //Nom donn� par l'utilisateur, Modifiable

        //Constructeur
        public FunctionalModel(int value, string name) 
        {
            dpt_value = value;
            Name = name;
        }

        //Methodes
        public override string ToString() //Fonction utilis� d�s que l'affchage d'un mod�le et demand�, utilis� pas la vue.
        {
            return $"M{Key} | {Name}";
        }

        public override bool Equals(object? obj) //Non utilis� mais verifie l'unicit� d'un mod�le, venir modifier en cas d'ajout d'attributs
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
            return dpt_value == other.dpt_value && Name == other.Name && Key == other.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dpt_value, Name, Key);
        }

    }
}

