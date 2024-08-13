using Xunit;
using KNXIntegrator;

namespace KNXIntegrator.Models.UnitTests;

public class GrpAddrDictionnaryTests{

    [Fact]
    public void GetElementFromDict(){
        //Arrange
        var dict = new GrpAddrDictionnary();

        //Act
        var addr1 = dict.GetGrpAddr("Cmd");
        var addr2 = dict.getGrpAddr("Ie");

        //Assert
        Assert.Equal("", addr);
        throw new Exception("je n'ai pas fini d'écrire ce test");
    }

}
