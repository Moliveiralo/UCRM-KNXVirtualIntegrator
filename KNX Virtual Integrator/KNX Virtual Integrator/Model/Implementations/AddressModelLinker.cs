using Knx.Falcon.ApplicationData.MasterData;
using Knx.Falcon;
using KNX_Virtual_Integrator.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KNX_Virtual_Integrator.Model.Interfaces;

namespace KNX_Virtual_Integrator.Model.Implementations
{
    public class AddressModelLinker : IAddressModelLinker
    {
        private IGroupAddressManager _addrManager;
        private readonly Dictionary<string, List<XElement>> _groupedAddresses;
        private readonly Dictionary<int, FunctionalModel> _models;

        public AddressModelLinker(IGroupAddressManager addrManager, Dictionary<int, FunctionalModel> models) /*(Dictionary<string, List<XElement>> groupedAddresses, Dictionary<int, FunctionalModel> models)*/
        {
            //_groupedAddresses = groupedAddresses;
            _models = models;
            _addrManager = addrManager;
            _groupedAddresses = addrManager.GetGroupedAddrDict();
        }

        public async Task<Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)>> CreateInformationsGlobAsync()
        {
            var informationsglob = new Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)>();

            foreach (var entry in _groupedAddresses)
            {
                var commonName = entry.Key;
                var infos = entry.Value;

                var cmdAddressElement = infos.FirstOrDefault(a => a.Attribute("Name")?.Value.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase) == true); //identifier ou est l'@ cmd
                var ieAddressElements = infos.Where(a => a.Attribute("Name")?.Value.StartsWith("Ie", StringComparison.OrdinalIgnoreCase) == true).ToList(); //identifier ou sont les @ ie

                if (cmdAddressElement != null /*&& ieAddressElements != null*/)
                {
                    //attention sinspirer de daichi !!!!!
                    var cmdAddress = cmdAddressElement.Attribute("Address")?.Value;
                    var dpts = cmdAddressElement.Attribute("DPTs")?.Value;
                    /*var dptslecture = ieAddressElements.Attribute("DPTs")?.Value; //revoir parce que liste*/

                    if (cmdAddress != null && dpts != null /*&& dptslecture != null*/)
                    {
                        // Extraire les DPT et SubDPT en cmd
                        var dptParts = dpts.Split('-');
                        if (dptParts.Length == 3) // 0, 1 et 2 = 3
                        {
                            var mainTypeNumber = int.Parse(dptParts[1]);
                            var subtypeNumber = int.Parse(dptParts[2]);

                            // Vérification type
                            if (_models.TryGetValue(mainTypeNumber, out var model))
                            {
                                // Vérification sous-type
                                if (model.SubDPT.SubTypeNumber == subtypeNumber) //pas sur de moi
                                {
                                    var ieAddress = ieAddressElements.FirstOrDefault()?.Attribute("Address")?.Value;
                                    var key = $"{commonName}_{cmdAddress}";

                                    /*if (ieAddress == null) {
                                        informationsglob[key] = (
                                        CmdAddress: cmdAddress,
                                        IeAddress: new GroupAddress("0/0/0"),
                                        Dpt: model.DptValue,
                                        SubDpt: model.SubDPT,
                                        SizeInBit: model.SizeInBit,
                                        Ve: model.Ve,
                                        Va: model.Va
                                    );
                                    }
                                    else {*/
                                        informationsglob[key] = (
                                        CmdAddress: cmdAddress,
                                        IeAddress: ieAddress,
                                        Dpt: model.DptValue,
                                        SubDpt: model.SubDPT,
                                        SizeInBit: model.SizeInBit,
                                        Ve: model.Ve,
                                        Va: model.Va
                                    );
                                    //}
                                    
                                }
                            }
                        }

                        // Extraire les DPT et SubDPT en cmd
                        /*var dptlectureParts = dptslecture.Split('-');
                        if (dptlectureParts.Length == 3) // 0, 1 et 2 = 3
                        {
                            var mainTypeNumberlec = int.Parse(dptlectureParts[1]);
                            var subtypeNumberlec = int.Parse(dptlectureParts[2]);

                        }*/
                    }
                }
            }

            return informationsglob;
        }

        public async Task<List<string>> GetFormattedLinkResultAsync()
        {
            // Obtenir les informations globales de la méthode CreateInformationsGlobAsync
            var informationsglob = await CreateInformationsGlobAsync();

            // Préparer la liste de chaînes de caractères pour le résultat formaté
            var result = new List<string>();

            // Pour chaque modèle, ajouter les détails au résultat
            foreach (var modelEntry in _models)
            {
                var modelId = modelEntry.Key;
                var model = modelEntry.Value;

                // Ajouter les détails du modèle
                result.Add($"Modèle ID: {modelId}");
                result.Add($"DPT: {model.DptValue}");
                result.Add($"SubDPT: {model.SubDPT}");
                result.Add($"SizeInBit: {model.SizeInBit}");
                result.Add($"Ve: {model.Ve}");
                result.Add($"Va: {model.Va}");
                result.Add("Adresses et Infos Globales:");

                // Trouver les adresses associées au modèle
                var modelAddresses = informationsglob.Where(info => info.Value.Dpt == model.DptValue && info.Value.SubDpt == model.SubDPT);

                foreach (var addressEntry in modelAddresses)
                {
                    var key = addressEntry.Key;
                    var info = addressEntry.Value;

                    // Ajouter les détails de l'adresse
                    result.Add($"  - ADDRESS: {key}");
                    result.Add($"    CmdAddress: {info.CmdAddress}");
                    result.Add($"    IeAddress: {info.IeAddress}");
                    result.Add($"    SizeInBit: {info.SizeInBit}");
                    result.Add($"    Ve: {info.Ve}");
                    result.Add($"    Va: {info.Va}");
                }

                // Ajouter une ligne vide pour séparer les modèles
                result.Add(string.Empty);
            }

            return result;
        }


        public async Task<string> GetFormattedLinkResultInString()
        {
            List<string> list = await GetFormattedLinkResultAsync();
            string result = "";
            foreach (string str in list)
            {
                result += str + "\n";
            }

            return result;
        }


    }
}
