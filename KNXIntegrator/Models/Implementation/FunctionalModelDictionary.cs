using System;
using System.Linq;


namespace KNXIntegrator.Models;

//USE THE INTERFACE IFunctionalModelDictionnary instead
public class FunctionalModelDictionary : IFunctionalModelDictionary
{

    private Dictionary<string, FunctionalModel> functionalModels;

    public FunctionalModelDictionary()
    {
        functionalModels = new Dictionary<string, FunctionalModel>();
    }

    public void Add_FunctionalModel(string key, FunctionalModel FunctionalModel)
    {
        functionalModels.Add(key, FunctionalModel);
    }

    public void Remove_FunctionalModel(string key)
    {
        functionalModels.Remove(key);
    }

    public List<FunctionalModel> GetAllModels()
    {
        List<FunctionalModel> rslt = new List<FunctionalModel>();
        foreach (var model in functionalModels)
        {
            rslt.Add(model.Value);
        }
        return rslt;
    }

    public FunctionalModel Get_FunctionalModel(string key)
    {
        return functionalModels[key];
    }



    
}