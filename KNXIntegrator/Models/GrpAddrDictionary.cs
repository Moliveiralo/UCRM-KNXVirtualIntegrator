namespace KNXIntegrator.Models;

using System.Xml.Linq;
public class GrpAddrDictionary:IGrpAddrDictionary{
    private Dictionary<string,List<XElement>> dictionary;

    public GrpAddrDictionary(){

        dictionary = new DummyGroupAddressDictionary().dictionary;
    }

    public List<XElement> GetGrpAddr(string key){
        return dictionary[key];
    }


}