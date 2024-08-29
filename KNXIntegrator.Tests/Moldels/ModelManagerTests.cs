using Xunit;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using Knx.Falcon;
using Moq;

namespace KNXIntegrator.Models.IntegrationTests;

public class ModelManagerTests
{
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




    // Helper method to create mock XElement
    private XElement CreateGroupAddress(string id, string name, string address)
    {
        return new XElement("GroupAddress",
            new XAttribute("Id", id),
            new XAttribute("Name", name),
            new XAttribute("Address", address)
        );
    }

    // Create mock data for _groupedAddresses
    private Dictionary<string, List<XElement>> CreateMockGroupedAddresses()
    {
        var mockGroupedAddresses = new Dictionary<string, List<XElement>>();

        // Define some mock group addresses
        var address1 = CreateGroupAddress("GA-001", "CmdExample1", "1/1/1");
        var address2 = CreateGroupAddress("GA-002", "IeExample1", "1/1/2");
        var address3 = CreateGroupAddress("GA-003", "CmdExample2", "1/1/3");
        var address4 = CreateGroupAddress("GA-004", "IeExample2", "1/1/4");

        // Add mock entries to the dictionary
        mockGroupedAddresses["Example1_1/1/1"] = new List<XElement> { address1, address2 };
        mockGroupedAddresses["Example2_1/1/3"] = new List<XElement> { address3, address4 };

        return mockGroupedAddresses;
    }

    [Fact]
    public async void GeneralWalkthrough()
    {
        //Arrange
        ITransfer transfer = new Transfer();
        Dictionary<string, List<XElement>> mockAddresses = CreateMockGroupedAddresses();
        var mockComm = new Mock<IGroupCommunication>();

        mockComm
        .Setup(gc => gc.WriteListAsync(It.IsAny<List<(GroupAddress addr, GroupValue val)>>()))
        .Return(Task.CompletedTask);

        mockComm
        .Setup(gc => gc.ReadListAsync(It.IsAny<List<GroupAddress>>()))
        .ReturnsAsync(new List<(GroupAddress, GroupValue)>{
            (new GroupAddress(2),new GroupValue(false)),
            (new GroupAddress(2),new GroupValue(true))
        });






        //     //Act


        // var testResult = new List<(GroupAddress, GroupAddress, bool)>();
        //     foreach (var kvp in mockAddresses)
        //     {
        //         List<(GroupAddress,GroupValue)> toSend = transfer.FrameToSend(GroupAddress.Parse(kvp.Value[0].Attribute("Address").Value), 1,GroupAddress.Parse(kvp.Value[1].Attribute("Address").Value), 1);
        //         mockComm.WriteListAsync(toSend);
        //         List<GroupAddress> stateAddr =GroupAddress.Parse(kvp.Value[1].Attribute("Address").Value);
        //         var received = await mockComm.Object.ReadListAsync(stateAddr);
        //         var toReceive = transfer.FrameToReceive(GroupAddress.Parse(kvp.Value[0].Attribute("Address").Value), 1,GroupAddress.Parse(kvp.Value[0].Attribute("Address").Value), 1);
        //         testResult.AddRange(transfer.Analyze(toReceive, received));
        //     }

        //     //Assert
        //     var expected = new List<(GroupAddress, GroupAddress, bool)>{
        //         (new GroupAddress(1),new GroupAddress(2), true)
        //     };
        //     Assert.Equal(expected, testResult);



        //Act
        var testTable = new List<(GroupAddress,GroupValue,GroupAddress,GroupValue?,bool?)>();
        foreach (var kvp in mockAddresses)
        {
            GroupAddress cmdAddr = kvp.Value[0].Attribute("Address").Value;
            GroupAddress stateAddr = kvp.Value[1].Attribute("Address").Value;

            List<(GroupAddress,GroupValue,GroupAddress,GroupValue?,bool?)> records = transfer.GetRecords(cmdAddr,stateAddr);
            foreach((GroupAddress cmdAddr,GroupValue cmdVal,GroupAddress stateAddr, GroupValue? stateVal, bool? testOK) record in records){
                mockComm.Object.WriteAsync(cmdAddr, cmdVal);
                stateVal = mockComm.Object.ReadAsync(stateAddr);
                testOK = transfer.check(cmdValue,stateValue);
            }
            testTable.AddRange(records);

        }





    }



}