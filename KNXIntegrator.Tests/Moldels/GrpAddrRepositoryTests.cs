using Xunit;
using KNXIntegrator;
using System.Xml.Linq;

namespace KNXIntegrator.Models.UnitTests;

public class GrpAddrRepositoryTests{

    [Fact]
    public void GetElementFromDict(){
        //Arrange
        var dict = new GrpAddrRepository();
        var expected_addr1 = new List<XElement>
                {                 
                    new XElement("GroupAddress", new XAttribute("Id", "GA-001"), new XAttribute("Name", "IeLightLivingRoom")),
                    new XElement("GroupAddress", new XAttribute("Id", "GA-001"), new XAttribute("Name", "CmdLightLivingRoom"))
                };

        var expected_addr2 =new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-012"), new XAttribute("Name", "IeLightBedroom")),
                    new XElement("GroupAddress", new XAttribute("Id", "GA-002"), new XAttribute("Name", "CmdLightBedroom"))
                };

        //Act
        var actual_addr1 = dict.GetGrpAddr("LightLivingRoom");
        var actual_addr2 = dict.GetGrpAddr("LightBedroom");

        //Assert
        Assert.Fail("ce test n'est pas bien ecrit donc pas grave");

    }

}
