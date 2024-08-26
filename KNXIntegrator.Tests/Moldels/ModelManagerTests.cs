using Xunit;

namespace KNXIntegrator.Models.IntegrationTests;

public class ModelManagerTests{
    // private IKeyPairDatabase keysDB = new KeyPairDatabase();
    // private IGrpAddrRepository addrRepo = new GrpAddrRepository();
    // private FunctionalModelDictionary funDict = new FunctionalModelDictionary();

    // private DummyGroupCommunication:IGroupCommunication{
    //     //TODO
    // }

    // public ModelManagerTests(){
    //     //simulate creation of models by user
    //     var model1 = new FunctionalModel("key1",1);
    //     var model2 = new FunctionalModel("key2",2);
    //     funDict.Add_FunctionalModel(model1);
    //     funDict.Add_FunctionalModel(model2);

    //     //simulate the mapping between models and addresses
    //     keysDB.Add("key1","LightLivingRoom");
    //     keysDB.Add("key2","LightBedroom");

    //     //given a Cmd "LightLivingRoom", what IE should be returned ?
    //     var myGrpAddr = addrRepo.GetGrpAddr("LightLivingRoom");
    //     var myModelKey = keysDB.GetByKey2("LightLivingRoom");
    //     var myModel = funDict.Get_FunctionalModel(myModelKey);
    //     var commands = Transfer.GetSendCommands(myGrpAddr.addrInt,myModel);
    //     var expected = Transfer.GetExpectedCommands(myGrpAddr.addrInt,myModel);

    //     IGroupCommunication comm = new DummyGroupCommunication();
    //     await comm.WriteAsync(commands);
    //     var actual = await comm.ReadAsync(source_addr : myGrpAddr.addrInt);
    //     foreach(var frame in expected){
    //         Assert.Equals(frame,actual.GetByAddr(frame.addr));
    //     }



    // }
    

}