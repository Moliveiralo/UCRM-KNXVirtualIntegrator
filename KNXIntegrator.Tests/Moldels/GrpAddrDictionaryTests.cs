using Xunit;
using KNXIntegrator;

namespace KNXIntegrator.Models.UnitTests;

public class GrpAddrDictionaryTests{

    [Fact]
    public void GetElementFromDict(){
        //Arrange
        var dict = new GrpAddrDictionary();

        //Act
        var addr1 = dict.GetGrpAddr("Cmd");
        var addr2 = dict.GetGrpAddr("Ie");

        //Assert
        Assert.True(false,"test not implemented yet");

    }

}
