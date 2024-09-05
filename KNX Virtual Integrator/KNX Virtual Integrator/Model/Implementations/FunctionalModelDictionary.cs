using System;
using System.Linq;


namespace KNXIntegrator.Models
{
    //Find Summary in the interface
    public class FunctionalModelDictionary : IFunctionalModelDictionary
    {
        private Dictionary<int, FunctionalModel> functionalModels;
        private int _currentKey;

        public FunctionalModelDictionary()
        {
            functionalModels = new Dictionary<int, FunctionalModel>();
            _currentKey = 0; // Commence � 0 pour que la premi�re cl� soit 1
        }

        public void Add_FunctionalModel(FunctionalModel functionalModel)
        {
            _currentKey++;
            functionalModel.Key = _currentKey;  // Associer la cl� au mod�le
            functionalModels.Add(_currentKey, functionalModel);
        }

        public void Remove_FunctionalModel(int key)
        {
            functionalModels.Remove(key);
        }

        public List<FunctionalModel> GetAllModels()
        {
            return new List<FunctionalModel>(functionalModels.Values);
        }

    }
}

