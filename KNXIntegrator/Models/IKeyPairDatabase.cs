namespace Models;

public interface IKeyPairDatabase{

    public void Add(string key1, string key2);

    public void Remove(string key1, string key2);

    public string GetByKey1(string key1);

    public string GetByKey2(string key2);
    
}