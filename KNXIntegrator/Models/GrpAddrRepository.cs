namespace KNXIntegrator.Models;

using System.Xml.Linq;
public class GrpAddrRepository:IGrpAddrRepository{
    private Dictionary<string,List<XElement>> dictionary;

    public GrpAddrRepository(){

        dictionary = new DummyGroupAddressDictionary().dictionary;
    }

    public List<XElement> GetGrpAddr(string key){
        return dictionary[key];
    }


}