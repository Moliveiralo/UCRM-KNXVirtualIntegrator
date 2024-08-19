using xunit;

namespace KNXIntegrator.Models.IntegrationTests;

public class ModelManagerTests{
    private IKeyPairDatabase keysDB = new KeyPairDatabase();
    private IGrpAddrRepository addrRepo = new GrpAddrRepository();
    private FunctionalModelDictionary funDict = new FunctionalModelDictionary();

}