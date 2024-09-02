// using Xunit;
// using System.Collections.Generic;
// using System.Xml.Linq;
// using System.Linq;
// using System.Threading.Tasks;
// using Knx.Falcon;
// using Moq;
//
// namespace KNXIntegrator.Models.IntegrationTests;
//
// public class GeneralTest
// {
//     
//
//     [Fact]
//     public async void GeneralWalkthrough()
//     {
//         //Arrange
//         IAnalysis analyzer = new Analysis();
//         Dictionary<string, List<XElement>> addresses = CreateMockGroupedAddresses();
//         IGroupCommunication comm = new GroupCommunication();
//
//         mockComm
//             .Setup(gc => gc.WriteAsync(It.IsAny<(GroupAddress addr, GroupValue val)>()))
//             .Returns(Task.CompletedTask);
//
//         mockComm
//             .Setup(gc => gc.ReadAsync(It.IsAny<GroupAddress>()))
//             .ReturnsAsync(new GroupValue(true));
//
//
//         //Act
//         var testTable = new List<Analysis.RecordEntry>();
//         foreach (var kvp in mockAddresses)
//         {
//             List<Analysis.RecordEntry> records = analyzer.GetRecords(
//                 new GroupAddress(kvp.Value[0].Attribute("Address").Value),
//                 new GroupAddress(kvp.Value[1].Attribute("Address").Value), 1);
//             for (int i = 0; i < records.Count; i++)
//             {
//                 mockComm.Object.WriteAsync((records[i].CmdAddr, records[i].CmdVal));
//                 records[i].StateVal = await mockComm.Object.ReadAsync(records[i].StateAddr);
//                 records[i].TestOK = analyzer.Check(records[i].CmdVal, records[i].StateVal);
//             }
//
//             testTable.AddRange(records);
//         }
//
//         //Assert
//         var expected = new List<Analysis.RecordEntry>
//         {
//             new Analysis.RecordEntry
//             {
//                 CmdAddr = new GroupAddress("1/1/1"),
//                 CmdVal = new GroupValue(false),
//                 StateAddr = new GroupAddress("1/1/2"),
//                 StateVal = new GroupValue(true),
//                 TestOK = false
//             },
//             new Analysis.RecordEntry
//             {
//                 CmdAddr = new GroupAddress("1/1/1"),
//                 CmdVal = new GroupValue(true),
//                 StateAddr = new GroupAddress("1/1/2"),
//                 StateVal = new GroupValue(true),
//                 TestOK = true
//             },
//             new Analysis.RecordEntry
//             {
//                 CmdAddr = new GroupAddress("1/1/3"),
//                 CmdVal = new GroupValue(false),
//                 StateAddr = new GroupAddress("1/1/4"),
//                 StateVal = new GroupValue(true),
//                 TestOK = false
//             },
//             new Analysis.RecordEntry
//             {
//                 CmdAddr = new GroupAddress("1/1/3"),
//                 CmdVal = new GroupValue(true),
//                 StateAddr = new GroupAddress("1/1/4"),
//                 StateVal = new GroupValue(true),
//                 TestOK = true
//             }
//         };
//         Assert.Equal(expected, testTable);
//     }
// }