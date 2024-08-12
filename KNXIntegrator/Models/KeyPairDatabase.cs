using models;

namespace Models;

public class KeyPairDatabase:IKeyPairDatabase{

    private List<(string key1,string key2)> keys = new List<(string, string)>();


    public void Add(string key1, string key2){
        keys.Add((key1,key2));
    }

    public void Remove(string key1, string key2){
        keys.Remove((key1,key2));
    }


    public string GetByKey1(string key1){
        var result = keys.Where(k => k.key1 == key1)
                         .Select(k => k.key2)
                         .FirstOrDefault(); // Use FirstOrDefault to handle cases where key1 is not found

        return result;
    }

    public string GetByKey2(string key2){
                var result = keys.Where(k => k.key2 == key2)
                         .Select(k => k.key1)
                         .FirstOrDefault(); // Use FirstOrDefault to handle cases where key1 is not found

        return result;

    }


}