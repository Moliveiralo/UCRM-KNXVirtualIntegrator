// using System.Xml.Linq;
// using System.IO;
// using System;
//
// namespace KNXIntegrator.Models.UnitTests;
//
// public class GrpAddrRepositoryTests
// {
//     
//     
//     // Helper method to create mock XElement
//     private XElement CreateGroupAddress(string name, string address, string dpt, string xmlns)
//     {
//         return new XElement("GroupAddress",
//             new XAttribute("Name", name),
//             new XAttribute("Address", address),
//             new XAttribute("DPT", dpt),
//             new XAttribute("xmlns", xmlns)
//             
//         );
//     }
//
//     // Create mock data for _groupedAddresses
//     private Dictionary<string, List<XElement>> CreateMockGroupedAddresses()
//     {
//         var mockGroupedAddresses = new Dictionary<string, List<XElement>>();
//
//         // Define some mock group addresses
//         var address1 = CreateGroupAddress("Cmd_Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Cuisine", "1/1/1","DPST-1-1","http://knx.org/xml/ga-export/01");
//         var address2 = CreateGroupAddress("Ie_Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Cuisine", "1/1/2","DPST-1-1","http://knx.org/xml/ga-export/01");
//         var address3 = CreateGroupAddress("Cmd_Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Salon", "1/1/3","DPST-1-1","http://knx.org/xml/ga-export/01");
//         var address4 = CreateGroupAddress("Ie_Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Salon", "1/1/4","DPST-1-1","http://knx.org/xml/ga-export/01");
//
//         // Add mock entries to the dictionary
//         mockGroupedAddresses["Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Cuisine_1/1/1"] = new List<XElement> { address1, address2 };
//         mockGroupedAddresses["Eclairage_MarcheArret_MaisonMrDurant_FacadeXx_Rdc_Cuisine_1/1/3"] = new List<XElement> { address3, address4 };
//
//         return mockGroupedAddresses;
//     }
//
//     private string GetXmlPath()
//     {
//         string baseDirectory = Directory.GetCurrentDirectory();
//         string filePath = Path.Combine(baseDirectory, "../../../XmlFiles/GrpAddr.xml");
//         return filePath;
//     }
//     
//     [Fact]
//     public void GrpAddrRepositoryTestsOK()
//     {
//         //Arrange
//         var repo = new GrpAddrRepository();
//         
//         //Act
//         XDocument xmlDoc = XDocument.Load(GetXmlPath());
//         repo.ProcessStandardXmlFile(xmlDoc);
//         
//         //Assert
//         var expected = CreateMockGroupedAddresses();
//         Assert.Equal(expected, repo.GroupedAddresses);
//
//     }
//
//
// }