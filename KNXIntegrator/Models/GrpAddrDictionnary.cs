namespace Models;

public class GrpAddrDictionnary{
    private Dictionnarty<string,List<XElement>> dictionnary;

    public GrpAddrDictionnary(){
        dictionnary = new DummyGroupAddressDictionnary();
    }

    public List<XElement> GetGrpAddr(string key){
        return dictionnary[key];
    }


}