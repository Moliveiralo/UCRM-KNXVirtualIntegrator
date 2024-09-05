using System.Collections.Generic;

namespace KNXIntegrator.Models
{
    public interface IFunctionalModelDictionary
    {
        // Ajouter un mod�le au dictionnaire
        void Add_FunctionalModel(FunctionalModel functionalModel);

        // Supprimer un mod�le du dictionnaire par sa cl�
        void Remove_FunctionalModel(int key);

        // R�cup�rer tous les mod�les
        List<FunctionalModel> GetAllModels();

        // R�cup�rer un mod�le par sa cl�
        FunctionalModel Get_FunctionalModel(int key);

        // Mettre � jour un mod�le existant dans le dictionnaire
        void UpdateModel(FunctionalModel model);
    }
}

