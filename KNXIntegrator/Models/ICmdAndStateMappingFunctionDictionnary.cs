namespace Models;

public interface IFunctionalModelDictionnary{
    public void Add_FunctionalModel (string name, FunctionalModel functionalModel);

    public void Remove_FunctionalModel (string name);

    public void Show_FunctionalModels ();

    public FunctionalModel Get_FunctionalModel(string name);
}