using Knx.Falcon.ApplicationData.MasterData;
using Knx.Falcon;
using KNX_Virtual_Integrator.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KNX_Virtual_Integrator.Model.Implementations
{
    public class AddressModelLinker
    {
        private readonly Dictionary<string, List<XElement>> _groupedAddresses;
        private readonly Dictionary<int, FunctionalModel> _models;

        public AddressModelLinker(Dictionary<string, List<XElement>> groupedAddresses, Dictionary<int, FunctionalModel> models)
        {
            _groupedAddresses = groupedAddresses;
            _models = models;
        }

        public Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)> CreateInformationsGlob()
        {
            var informationsglob = new Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)>();

            foreach (var entry in _groupedAddresses)
            {
                var commonName = entry.Key;
                var addresses = entry.Value;

                //a quoi ca sert ?
                var cmdAddressElement = addresses.FirstOrDefault(a => a.Attribute("Name")?.Value.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase) == true);
                var ieAddressElements = addresses.Where(a => a.Attribute("Name")?.Value.StartsWith("Ie", StringComparison.OrdinalIgnoreCase) == true).ToList();

                if (cmdAddressElement != null)
                {
                    //attention sinspirer de daichi !!!!!
                    var cmdAddress = cmdAddressElement.Attribute("Address")?.Value;
                    var dpts = cmdAddressElement.Attribute("DPTs")?.Value;

                    if (cmdAddress != null && dpts != null)
                    {
                        // Extraire les DPT et SubDPT
                        var dptParts = dpts.Split('-');
                        if (dptParts.Length == 3)
                        {
                            var mainTypeNumber = int.Parse(dptParts[1]);
                            var subtypeNumber = int.Parse(dptParts[2]);

                            if (_models.TryGetValue(mainTypeNumber, out var model))
                            {
                                var ieAddress = ieAddressElements.FirstOrDefault()?.Attribute("Address")?.Value;
                                var key = $"{commonName}_{cmdAddress}";

                                informationsglob[key] = (
                                    CmdAddress: cmdAddress,
                                    IeAddress: ieAddress,
                                    Dpt: model.DptValue,
                                    SubDpt: model.SubDPT,
                                    SizeInBit: model.SizeInBit,
                                    Ve: model.Ve,
                                    Va: model.Va
                                );
                            }
                        }
                    }
                }
            }

            return informationsglob;
        }
    }
}
