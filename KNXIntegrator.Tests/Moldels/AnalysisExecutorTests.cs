using System.Collections;
using Xunit;
using Knx.Falcon;
using System.Xml.Linq;
using Moq;

namespace KNXIntegrator.Models.UnitTests;

public class AnalysisExecutorTests
{
    
    // Helper method to create mock XElement
    private static XElement CreateGroupAddress(string id, string name, string address)
    {
        return new XElement("GroupAddress",
            new XAttribute("Id", id),
            new XAttribute("Name", name),
            new XAttribute("Address", address)
        );
    }

    // Create mock data for _groupedAddresses
    private static Dictionary<string, List<XElement>> CreateMockGroupedAddresses()
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

    private static Mock<IGroupCommunication> CreateMockCommunication()
    {
        var mockComm = new Mock<IGroupCommunication>();

        mockComm
            .Setup(gc => gc.WriteAsync(It.IsAny<(GroupAddress addr, GroupValue val)>()))
            .Returns(Task.CompletedTask);

        mockComm
            .Setup(gc => gc.ReadAsync(It.IsAny<GroupAddress>()))
            .ReturnsAsync(new GroupValue(true));

        return mockComm;
    }
    
    // ClassData provider for the theory
    public class AnalysisExecutorTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var mockComm = CreateMockCommunication();
            var mockGroupedAddresses = CreateMockGroupedAddresses();
            var analyzer = new Mock<IAnalysis>().Object;

            yield return new object[] { mockComm.Object, mockGroupedAddresses, analyzer };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    
    
    
    [Theory]
    [ClassData(typeof(AnalysisExecutorTestData))]
    public async void AnalysisExecutionOK(IGroupCommunication comm,Dictionary<string, List<XElement>> addrDict,IAnalysis analyzer)
    {
        //Arrange
        IAnalysisExecutor executor = new AnalysisExecutor(comm,addrDict,analyzer);
        
        //Act

        List<Analysis.RecordEntry> results = await executor.RunAndGetResults();


        //Assert
        var expected = new List<Analysis.RecordEntry>
        {
            new Analysis.RecordEntry
            {
                CmdAddr = new GroupAddress("1/1/1"),
                CmdVal = new GroupValue(false),
                StateAddr = new GroupAddress("1/1/2"),
                StateVal = new GroupValue(true),
                TestOK = false
            },
            new Analysis.RecordEntry
            {
                CmdAddr = new GroupAddress("1/1/1"),
                CmdVal = new GroupValue(true),
                StateAddr = new GroupAddress("1/1/2"),
                StateVal = new GroupValue(true),
                TestOK = true
            },
            new Analysis.RecordEntry
            {
                CmdAddr = new GroupAddress("1/1/3"),
                CmdVal = new GroupValue(false),
                StateAddr = new GroupAddress("1/1/4"),
                StateVal = new GroupValue(true),
                TestOK = false
            },
            new Analysis.RecordEntry
            {
                CmdAddr = new GroupAddress("1/1/3"),
                CmdVal = new GroupValue(true),
                StateAddr = new GroupAddress("1/1/4"),
                StateVal = new GroupValue(true),
                TestOK = true
            }
        };
        Assert.Equal(expected,results);

        // var expectedStr = new List<string>
        // {
        //     ("CmdAddr = " + "1/1/1" + "\nCmdVal = " + "false" + "\nStateAddr = " +
        //      "1/1/2" + "\nStateVal = " + "true" + "\nTestOK = " + "false" +
        //      "\n\n"),
        //     ("CmdAddr = " + "1/1/1" + "\nCmdVal = " + "true" + "\nStateAddr = " +
        //      "1/1/2" + "\nStateVal = " + "true" + "\nTestOK = " + "true" +
        //      "\n\n"),
        //
        //     ("CmdAddr = " + "1/1/3" + "\nCmdVal = " + "false" + "\nStateAddr = " +
        //      "1/1/4" + "\nStateVal = " + "true" + "\nTestOK = " + "false" +
        //      "\n\n"),
        //
        //     ("CmdAddr = " + "1/1/3" + "\nCmdVal = " + "true" + "\nStateAddr = " +
        //      "1/1/4" + "\nStateVal = " + "true" + "\nTestOK = " + "true" +
        //      "\n\n")
        // };
        // Assert.Equal(expectedStr,resultStr);


    }

    
}