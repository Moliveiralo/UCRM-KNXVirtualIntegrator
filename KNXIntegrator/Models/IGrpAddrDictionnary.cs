using System.Xml.Linq;

namespace Models;

public interface IGrpAddrDictionnary{

    public List<XElement> GetGrpAddr(string key);
    
}