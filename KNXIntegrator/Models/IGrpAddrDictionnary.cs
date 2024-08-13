using System.Xml.Linq;

namespace KNXIntegrator.Models;

public interface IGrpAddrDictionary{

    public List<XElement> GetGrpAddr(string key);
    
}