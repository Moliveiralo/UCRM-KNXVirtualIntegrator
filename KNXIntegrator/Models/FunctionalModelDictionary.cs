using System;


namespace KNXIntegrator.Models;

//USE THE INTERFACE IFunctionalModelDictionnary instead
public class FunctionalModelDictionary : IFunctionalModelDictionary{

    private Dictionary<string,FunctionalModel> functionalModels;

    public FunctionalModelDictionary(){
        functionalModels = new Dictionary<string, FunctionalModel>();
    }

    public void Add_FunctionalModel (string name, FunctionalModel FunctionalModel){
        functionalModels.Add(name,FunctionalModel);
    }

    public void Remove_FunctionalModel (string name){
        functionalModels.Remove(name);
    }

    public void Show_FunctionalModels (){
        Console.WriteLine("lits of KNX FunctionalModels : \n");
        foreach (var kvp in functionalModels){
            Console.WriteLine(kvp.Key+" - "+kvp.Value.ToString());
        }
    }

    public FunctionalModel Get_FunctionalModel(string name){
        return functionalModels[name];
    }

    public static void Tests(){
        FunctionalModelDictionary dict = new FunctionalModelDictionary();
        FunctionalModel functionalModel1 = new FunctionalModel(1);
        dict.Add_FunctionalModel("FunctionalModel1",functionalModel1);
        dict.Show_FunctionalModels();
        Console.WriteLine("FunctionalModel1 value = "+dict.Get_FunctionalModel("FunctionalModel1").ToString()+"\n");
        

    }
}