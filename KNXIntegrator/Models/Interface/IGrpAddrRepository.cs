using System.Xml.Linq;

namespace KNXIntegrator.Models;

public interface IGrpAddrRepository{

    public List<XElement> GetGrpAddr(string key);
    
}