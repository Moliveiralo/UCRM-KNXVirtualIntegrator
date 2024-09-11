using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Knx.Falcon.ApplicationData.MasterData;
using Knx.Falcon.ApplicationData;
using Knx.Falcon.ApplicationData.DatapointTypes;
using Knx.Falcon.ApplicationData.PropertyDataTypes;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Entities
{
    
    public class ModelDictionary
    {
        
        public static Dictionary<int, FunctionalModel> GetDefaultModels()
        {
            // Créer une instance de DptFactory
            var dptFactory = DptFactory.Default;

            var _allDatapointTypes = DptFactory.Default.AllDatapointTypes;

            // Récupérer le DatapointType et DatapointSubtype
            //var dptTypef = dptFactory.GetDatapointType(1);   
            //var dptSubtypef = dptFactory.GetDatapointSubtype(1, 1);

            // Trouver un type de point de données spécifique (par exemple, le type avec le numéro 1)
            var dptTypeo = _allDatapointTypes.FirstOrDefault(t => t.MainTypeNumber == 1);
            if (dptTypeo == null)
                throw new Exception("DatapointType avec le numéro 1 non trouvé.");

            // Récupérer un sous-type spécifique (par exemple, le sous-type avec le numéro 1 pour le type 1)
            var dptSubtypeo = dptFactory.GetDatapointSubtype(1, 1);

            var dptTyped = dptFactory.GetDatapointType(5);
            var dptSubtyped = dptFactory.GetDatapointSubtype(5, 1);

            var dptTypet = dptFactory.GetDatapointType(1);
            var dptSubtypet = dptFactory.GetDatapointSubtype(1, 8);

            if (dptSubtypeo == null)
                throw new Exception("DatapointSubtype avec le numéro 1 pour le type 1 non trouvé.");

            // Créer le dictionnaire de modèles
            return new Dictionary<int, FunctionalModel>
            {
                { 1, new FunctionalModel(
                        dptTypeo,               // Initialisation avec le DPT récupéré
                        "Modèle ON/OFF",
                        dptSubtypeo,            // Initialisation avec le SubDPT récupéré
                        1,                     // 1bit
                        new GroupValue(true),                   //envoi 1 ON
                        new GroupValue(true))                   //lecture 1 ON
                },

                { 2, new FunctionalModel(
                        dptTyped,               // Initialisation avec le DPT récupéré
                        "Modèle ON/OFF",
                        dptSubtyped,            // Initialisation avec le SubDPT récupéré
                        1,                     // 1bit
                        new GroupValue(true),                   //envoi 1 ON
                        new GroupValue(true))                   //lecture 1 ON
                }
            };
        }
    }
}
