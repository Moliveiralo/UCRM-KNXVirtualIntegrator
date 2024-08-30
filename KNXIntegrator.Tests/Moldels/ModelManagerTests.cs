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
        IAnalysis analyzer = new Analysis();
        Dictionary<string, List<XElement>> mockAddresses = CreateMockGroupedAddresses();
        var mockComm = new Mock<IGroupCommunication>();

        mockComm
        .Setup(gc => gc.WriteAsync(It.IsAny<(GroupAddress addr, GroupValue val)>()))
        .Returns(Task.CompletedTask);

        mockComm
        .Setup(gc => gc.ReadAsync(It.IsAny<GroupAddress>()))
        .ReturnsAsync(new GroupValue(true));





        //Act
        var testTable = new List<Analysis.RecordEntry>();
        foreach (var kvp in mockAddresses)
        {

            List<Analysis.RecordEntry> records = analyzer.GetRecords(kvp.Value[0].Attribute("Address").Value,kvp.Value[1].Attribute("Address").Value,1);
            //foreach((GroupAddress cmdAddr,GroupValue cmdVal,GroupAddress stateAddr, GroupValue? stateVal, bool? testOK) in records){
            for(int i = 0;i<records.Count;i++){
                mockComm.Object.WriteAsync((records[i].CmdAddr, records[i].CmdVal));
                records[i].StateVal = mockComm.Object.ReadAsync(records[i].stateAddr);
                records[i].TestOK = analyzer.Check(records[i].CmdVal,records[i].StateVal);
            }
            testTable.AddRange(records);

        }





    }



}