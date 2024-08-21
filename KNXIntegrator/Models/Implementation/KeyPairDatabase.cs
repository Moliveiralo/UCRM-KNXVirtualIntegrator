

namespace KNXIntegrator.Models;

public class KeyPairDatabase:IKeyPairDatabase{

    public List<(string key1,string key2)> keys {get;private set;} = new List<(string, string)>(); 


    public void Add(string key1, string key2){
        keys.Add((key1,key2));
    }

    public void Remove(string key1, string key2){
        keys.Remove((key1,key2));
    }


    public List<string> GetByKey1(string key1){
        var result = keys.Where(k => k.key1 == key1)
                         .Select(k => k.key2)
                         .ToList();

        return result;
    }

    public List<string> GetByKey2(string key2){
                var result = keys.Where(k => k.key2 == key2)
                         .Select(k => k.key1)
                         .ToList();

        return result;

    }


}