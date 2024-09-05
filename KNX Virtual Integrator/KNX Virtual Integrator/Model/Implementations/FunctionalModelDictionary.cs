using System;
using System.Linq;


namespace KNXIntegrator.Models
{
    public class FunctionalModelDictionary : IFunctionalModelDictionary
    {
        private Dictionary<int, FunctionalModel> functionalModels;
        private int _currentKey;

        public FunctionalModelDictionary()
        {
            functionalModels = new Dictionary<int, FunctionalModel>();
            _currentKey = 0; // Commence � 0 pour que la premi�re cl� soit 1
        }

        // Ajouter un mod�le au dictionnaire
        public void Add_FunctionalModel(FunctionalModel functionalModel)
        {
            _currentKey++;
            functionalModel.Key = _currentKey;  // Associer la cl� au mod�le
            functionalModels.Add(_currentKey, functionalModel);
        }

        // Supprimer un mod�le du dictionnaire par sa cl�
        public void Remove_FunctionalModel(int key)
        {
            functionalModels.Remove(key);
        }

        // R�cup�rer tous les mod�les
        public List<FunctionalModel> GetAllModels()
        {
            return new List<FunctionalModel>(functionalModels.Values);
        }

        // R�cup�rer un mod�le par sa cl�
        public FunctionalModel Get_FunctionalModel(int key)
        {
            return functionalModels[key];
        }

        // Mettre � jour un mod�le existant dans le dictionnaire
        public void UpdateModel(FunctionalModel model)
        {
            if (model != null && functionalModels.ContainsKey(model.Key))
            {
                // Met � jour le mod�le existant dans le dictionnaire en utilisant la cl�
                functionalModels[model.Key] = model;
            }
            else
            {
                throw new KeyNotFoundException($"Le mod�le avec la cl� {model?.Key} n'existe pas dans le dictionnaire.");
            }
        }
    }
}

