using System.Collections.Generic;

namespace KNXIntegrator.Models
{
    public interface IFunctionalModelDictionary
    {
        // Ajoute un mod�le avec une cl� g�n�r�e automatiquement
        void Add_FunctionalModel(FunctionalModel functionalModel);

        // Supprime un mod�le en fonction de la cl�
        void Remove_FunctionalModel(int key);

        // R�cup�re une liste de tous les mod�les
        List<FunctionalModel> GetAllModels();

        // R�cup�re un mod�le sp�cifique par cl�
        FunctionalModel Get_FunctionalModel(int key);
    }
}
