using Xunit;
using KNXIntegrator.Models;
using Knx.Falcon.Sdk;

namespace KNXIntegrator.Models.UnitTests;

public class TransferTests {

    [Fact]
    public void IsCorrectSend(){
        //Arrange
        Transfer transfer = new Transfer();
        var expected = new List<(int addr,GroupValue value)>{
            (new GroupAddress(1),new GroupValue(0)),
            (new GroupAddress(1),new GroupValue(1))
            };
        
        //Act
        var actual = transfer.FrameToSend(cmdAddr: new GroupAddress(1),cmdDPT: new DptSimpleGeneric("1.001"),stateAddr: new GroupAddress(2), stateDPT: new DptSimpleGeneric("1.001"));

        //Assert
        Assert.Equal(expected,actual);
    }

    [Fact]
    public void IsCorrectReceive(){
        //Arrange
        Transfer transfer = new Transfer();
        var expected = new List<(int addr,GroupValue value)>{
            (new GroupAddress(2),new GroupValue(0)),
            (new GroupAddress(2),new GroupValue(1))
            };

        //Act
        var actual = transfer.FrameToReceive(cmdAddr: new GroupAddress(1),cmdDPT: new DptSimpleGeneric("1.001"),stateAddr: new GroupAddress(2), stateDPT: new DptSimpleGeneric("1.001"));

        //Assert
        Assert.Equal(expected,actual);
        

    }

    [Fact]
    public void IsCorrectAnalyze(){
        //Arrange
        Transfer transfer = new Transfer();
        var expected = new List<(GroupAddress cmdAddr,GroupAddress stateAddr,bool testOK)>(new GroupAddress(1),new GroupAddress(2),true);
        
        //Act
        //changer cet appel, voir interface
        var actual = transfer.Analyze(cmdAddr: new GroupAddress(1),cmdDPT: new DptSimpleGeneric("1.001"),stateAddr: new GroupAddress(2), stateDPT: new DptSimpleGeneric("1.001"));

        //Assert
        Assert.Equals(expected,actual);

    }
}

