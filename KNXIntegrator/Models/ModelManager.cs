

namespace KNXIntegrator.Models;

public class ModelManager{
    private IKeyPairDatabase keysDB = new KeyPairDatabase();
    private IGrpAddrRepository addrRepo = new GrpAddrRepository();
    private FunctionalModelDictionary funDict = new FunctionalModelDictionary();

}