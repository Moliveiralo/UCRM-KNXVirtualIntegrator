using Xunit;
using KNXIntegrator.Models;
using Knx.Falcon.Sdk;
using Knx.Falcon.ApplicationData.DatapointTypes;
using Knx.Falcon;


namespace KNXIntegrator.Models.UnitTests;

public class TransferTests {

    [Fact]
    public void IsCorrectSend(){
        //Arrange
        Transfer transfer = new Transfer();
        List<(GroupAddress addr, GroupValue value)> expected = new List<(GroupAddress addr,GroupValue value)>{
            (new GroupAddress(1),new GroupValue(false)),
            (new GroupAddress(1),new GroupValue(true))
            };
        
        //Act
        List<(GroupAddress addr, GroupValue value)> actual = transfer.FrameToSend(cmdAddr: new GroupAddress(1),1,stateAddr: new GroupAddress(2), 1);

        //Assert
        Assert.Equal(expected,actual);
    }

    [Fact]
    public void IsCorrectReceive(){
        //Arrange
        Transfer transfer = new Transfer();
        List<(GroupAddress addr, GroupValue value)> expected = new List<(GroupAddress addr,GroupValue value)>{
            (new GroupAddress(2),new GroupValue(false)),
            (new GroupAddress(2),new GroupValue(true))
            };

        //Act
        List<(GroupAddress addr, GroupValue value)> actual = transfer.FrameToReceive(cmdAddr: new GroupAddress(1),1,stateAddr: new GroupAddress(2), 1);

        //Assert
        Assert.Equal(expected,actual);
        

    }

    // [Fact]
    // public void IsCorrectAnalyze(){
    //     //Arrange
    //     Transfer transfer = new Transfer();
    //     var expected = new List<(GroupAddress cmdAddr,GroupAddress stateAddr,bool testOK)>{new GroupAddress(1),new GroupAddress(2),true};
        
    //     //Act
    //     var actual = transfer.Analyze(
    //         new List<(GroupAddress addr,GroupValue value)>{
    //             (new GroupAddress(1),new GroupValue(false)),
    //             (new GroupAddress(1),new GroupValue(true))
    //         },
    //         new List<(GroupAddress addr,GroupValue value)>{
    //             (new GroupAddress(1),new GroupValue(false)),
    //             (new GroupAddress(1),new GroupValue(true))
    //         }
    //         );

    //     //Assert
    //     Assert.Equals(expected,actual);

    // }
}

