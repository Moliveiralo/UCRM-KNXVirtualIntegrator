using Xunit;

namespace KNXIntegrator.Models.IntegrationTests;

public class ModelManagerTests{
    private IKeyPairDatabase keysDB = new KeyPairDatabase();
    private IGrpAddrRepository addrRepo = new GrpAddrRepository();
    private FunctionalModelDictionary funDict = new FunctionalModelDictionary();

    public ModelManagerTests(){
        //simulate creation of models by user
        var model1 = new FunctionalModel("key1",1);
        var model2 = new FunctionalModel("key2",2);
        funDict.Add_FunctionalModel(model1);
        funDict.Add_FunctionalModel(model2);

        //simulate the mapping between models and addresses
        keysDB.Add("key1","LightLivingRoom");
        keysDB.Add("key2","LightBedroom");

        //given a Cmd "LightLivingRoom", what IE should be returned ?
        var myGrpAddr = addrRepo.GetGrpAddr("LightLivingRoom");
        var myModelKey = keysDB.GetByKey2("LightLivingRoom");
        var myModel = funDict.Get_FunctionalModel(myModelKey);
        var commands = Transfer.GetSendCommands(myGrpAddr,myModel);
        var expected = Transfer.GetExpectedCommands(myGrpAddr,myModel);
        KNXManager.WriteFrames(commands);
        var actual = await KNXManager.ReadFrames(source_addr : myGrpAddr.addrHexa);
        foreach(frame in expected){
            Assert.Equals(frame,actual.Get(frame.addr));
        }



    }
    

}