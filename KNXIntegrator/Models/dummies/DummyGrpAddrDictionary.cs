using System.Collections.Generic;
using System.Xml.Linq;

namespace KNXIntegrator.Models
{
    public class DummyGroupAddressDictionary
    {
        // Field declaration with type specified
        public Dictionary<string, List<XElement>> dictionary { get; } = new Dictionary<string, List<XElement>>
        {
            {
                "LightLivingRoom", new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-011"), new XAttribute("Name", "IeLightLivingRoom")),
                    new XElement("GroupAddress", new XAttribute("Id", "GA-001"), new XAttribute("Name", "CmdLightLivingRoom"))
                }
            },
            {
                "LightBedroom", new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-012"), new XAttribute("Name", "IeLightBedroom")),
                    new XElement("GroupAddress", new XAttribute("Id", "GA-002"), new XAttribute("Name", "CmdLightBedroom"))
                }
            },
            {
                "HeatLivingRoom", new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-003"), new XAttribute("Name", "CmdHeatLivingRoom"))
                }
            },
            {
                "HeatBedroom", new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-004"), new XAttribute("Name", "CmdHeatBedroom"))
                }
            },
            {
                "LightKitchen", new List<XElement>
                {
                    new XElement("GroupAddress", new XAttribute("Id", "GA-005"), new XAttribute("Name", "IeLightKitchen"))
                }
            }
        };
    }
}
