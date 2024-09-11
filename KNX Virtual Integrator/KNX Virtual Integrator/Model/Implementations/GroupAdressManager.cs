﻿using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KNX_Virtual_Integrator.Model.Interfaces;

namespace KNX_Virtual_Integrator.Model.Implementations;

public class GroupAddressManager(Logger logger, ProjectFileManager projectFileManager, FileLoader loader, NamespaceResolver namespaceResolver, GroupAddressProcessor groupAddressProcessor, GroupAddressMerger groupAddressMerger) : IGroupAddressManager
{
    private readonly ILogger _logger = logger;

    public static XNamespace GlobalKnxNamespace = "http://knx.org/xml/ga-export/01";
    private readonly Dictionary<string, List<XElement>> _groupedAddresses = new ();
    private readonly List<XElement> _ieAddressesSet = new();

    public Dictionary<string, List<XElement>> GetGroupedAddrDict()
    {
        return _groupedAddresses;
    }

    /// <summary>
    /// Extracts group address information from a specified XML file.
    ///
    /// Determines the file path to use based on user input and whether a specific group address
    /// file is chosen or a default file is used. Depending on the file path, it processes the XML
    /// file to extract and group addresses either from a specific format or a standard format.
    /// </summary>
    public void ExtractGroupAddress()
    {
        if (projectFileManager is not { } manager) return;
        
        var filePath = App.WindowManager != null && App.WindowManager.MainWindow.UserChooseToImportGroupAddressFile
            ? manager.GroupAddressFilePath
            : manager.ZeroXmlPath;

        var groupAddressFile = loader.LoadXmlDocument(filePath);
        if (groupAddressFile == null) return;

        if (filePath == manager.ZeroXmlPath)
        {
            namespaceResolver.SetNamespaceFromXml(filePath);
            ProcessZeroXmlFile(groupAddressFile);
        }
        else
        {
            GlobalKnxNamespace = "http://knx.org/xml/ga-export/01";
            ProcessStandardXmlFile(groupAddressFile);
        }
    }

    /// <summary>
    /// Processes an XML file in the Zero format to extract and group addresses.
    ///
    /// This method extracts device references and their links, processes group addresses, and 
    /// groups them based on device links and common names. It handles the creation and updating 
    /// of grouped addresses, avoiding name collisions by appending suffixes if necessary.
    ///
    /// <param name="groupAddressFile">The XML document containing group address data in Zero format.</param>
    /// </summary>
    public void ProcessZeroXmlFile(XDocument groupAddressFile)
    {
        /// Initialiser le dictionnaire pour les adresses qui commencent par "Ie" avec un HashSet pour éviter les doublons
        var ieAddresses = new HashSet<string>();
        
        _ieAddressesSet.Clear();
        _groupedAddresses.Clear();

        int groupAddressStructure = DetermineGroupAddressStructure(groupAddressFile);
        
        // Étape 1 : Extraire les références des appareils
        var deviceRefs = groupAddressFile.Descendants(GlobalKnxNamespace + "DeviceInstance")
            .Select(di => (
                Id: di.Attribute("Id")?.Value,
                Links: di.Descendants(GlobalKnxNamespace + "ComObjectInstanceRef")
                    .Where(cir => cir.Attribute("Links") != null)
                    .SelectMany(cir => cir.Attribute("Links")?.Value.Split(' ') ?? Array.Empty<string>())
                    .ToHashSet()
            ))
            .ToList();

        var groupAddresses = groupAddressFile.Descendants(GlobalKnxNamespace + "GroupAddress").ToList();
        var tempGroupedAddresses = new Dictionary<(string CommonName, string DeviceId, string CmdAddress), HashSet<string>>();

        // Étape 2 : Regrouper les adresses par nom commun et ID de l'appareil
        foreach (var ga in groupAddresses)
        {
            var id = ga.Attribute("Id")?.Value;
            var name = ga.Attribute("Name")?.Value;
            // Convert the address to the x/x/x format (depending on the groupAddressStructure)
            var address = groupAddressProcessor.DecodeAddress(ga.Attribute("Address")?.Value ?? string.Empty, groupAddressStructure);
            
            // Extraire DPTs
            //var dpts = ga.Attribute("DPTs")?.Value;

            if (address != String.Empty)
            {
                ga.Attribute("Address")!.Value = address;
            }

            if (id == null || name == null) continue;

            var gaId = id.Contains("GA-") ? id.Substring(id.IndexOf("GA-", StringComparison.Ordinal)) : id;
            var linkedDevices = deviceRefs.Where(dr => dr.Links.Contains(gaId));

            foreach (var device in linkedDevices)
            {
                var isCmd = name.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase);
                var isIe = name.StartsWith("Ie", StringComparison.OrdinalIgnoreCase);
                var commonName = isCmd
                    ? name.Substring(3)
                    : isIe
                        ? name.Substring(2)
                        : name;

                // Seuls les "Cmd" sont pris en compte pour créer des groupes
                if (isCmd)
                {
                    var key = (CommonName: commonName, DeviceId: device.Id, CmdAddress: address);

                    if (!tempGroupedAddresses.ContainsKey(key))
                    {
                        tempGroupedAddresses[key] = new HashSet<string>();
                    }

                    tempGroupedAddresses[key].Add(id);
                }
                else if (name.StartsWith("Ie", StringComparison.OrdinalIgnoreCase))
                {
                    // Ajouter les "Ie" dans les groupes existants correspondants aux "Cmd"
                    var cmdKey = tempGroupedAddresses.Keys.FirstOrDefault(k => k.CommonName == commonName && k.DeviceId == device.Id);
                    if (cmdKey != default)
                    {
                        tempGroupedAddresses[cmdKey].Add(id);
                    }
                }
                
                // Ajouter les adresses "Ie" à la liste en vérifiant les doublons
                if (isIe)
                {
                    if (ieAddresses.Add(address)) // Essaie d'ajouter l'adresse, retourne vrai si l'adresse n'était pas déjà dans l'ensemble
                    {
                        _ieAddressesSet.Add(ga);
                    }
                }

                // Ajouter les DPTs aux éléments
                /*if (dpts != null)
                {
                    ga.SetAttributeValue("DPTs", dpts);
                }*/
            }
        }

        // Étape 3 : Regrouper les adresses "Cmd" et "Ie" sous la même clé, en tenant compte du DeviceId
        foreach (var entry in tempGroupedAddresses)
        {
            var commonName = $"{entry.Key.CommonName}_{entry.Key.CmdAddress}";
            var gaIds = entry.Value;

            // Chercher l'entrée existante basée sur le commonName et le DeviceId
            var existingEntry = _groupedAddresses.FirstOrDefault(g =>
                gaIds.All(id => g.Value.Any(x => x.Attribute("Id")?.Value == id)) ||
                g.Value.Select(x => x.Attribute("Id")?.Value).All(id => gaIds.Contains(id ?? string.Empty)));

            if (existingEntry.Value != null)
            {
                _logger.ConsoleAndLogWriteLine($"Matching or subset found for: {existingEntry.Key}. Adding missing IDs.");

                foreach (var gaId in gaIds)
                {
                    if (existingEntry.Value.All(x => x.Attribute("Id")?.Value != gaId))
                    {
                        var ga = groupAddresses.FirstOrDefault(x => x.Attribute("Id")?.Value == gaId);
                        if (ga != null) existingEntry.Value.Add(ga);
                    }
                }
            }
            else
            {
                _logger.ConsoleAndLogWriteLine($"Creating a new entry for: {commonName}");
                _groupedAddresses[commonName] = gaIds.Select(id => groupAddresses.First(x => x.Attribute("Id")?.Value == id)).ToList();
            }
        }

        // Étape 4 : Finaliser les regroupements sous les adresses "Cmd"
        foreach (var entry in _groupedAddresses)
        {
            var cmdAddress = entry.Value.FirstOrDefault(x => (bool)x.Attribute("Name")?.Value.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase));

            if (cmdAddress != null)
            {
                var commonName = entry.Key;
                _groupedAddresses[commonName] = new List<XElement> { cmdAddress };

                // Ajouter toutes les adresses "Ie" correspondantes sous le même nom commun
                _groupedAddresses[commonName].AddRange(entry.Value.Where(x => (bool)x.Attribute("Name")?.Value.StartsWith("Ie", StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                // Si aucune adresse "Cmd" n'est trouvée, ajouter le groupe tel quel
                _groupedAddresses[entry.Key] = entry.Value;
            }
        }
       
        groupAddressMerger.MergeSingleElementGroups(_groupedAddresses, _ieAddressesSet);
        
        
    }
    
    /// <summary>
    /// Processes an XML file in the standard format to extract and group addresses.
    ///
    /// This method processes group addresses from the XML file, normalizing the names by removing
    /// specific prefixes ("Ie" or "Cmd") and grouping addresses based on the remaining common names.
    ///
    /// <param name="groupAddressFile">The XML document containing group address data in standard format.</param>
    /// </summary>
    public void ProcessStandardXmlFile(XDocument groupAddressFile)
    {
        _ieAddressesSet.Clear();
        _groupedAddresses.Clear();
        var groupAddresses = groupAddressFile.Descendants(GlobalKnxNamespace + "GroupAddress").ToList();
    
        var ieAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
        var cmdAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
        var addedCmdAddresses = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        // Déclarez un HashSet pour suivre les clés des commandes déjà ajoutées
        var addedCommandKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


        foreach (var ga in groupAddresses)
        {
            var name = ga.Attribute("Name")?.Value;
            var address = ga.Attribute("Address")?.Value;
            //DPTs
            //var dpts = ga.Attribute("DPTs")?.Value;


            if (name != null && address != null)
            {
                //DPTs
                /*if (dpts != null)
                {
                    ga.SetAttributeValue("DPTs", dpts);
                }*/

                if (name.StartsWith("Ie", StringComparison.OrdinalIgnoreCase))
                {
                    var suffix = name.Substring(2);
                    // Vérifier si l'adresse est déjà présente dans la liste ieAddressesSet
                    if (!_ieAddressesSet.Any(x => x.Attribute("Address")?.Value == address))
                    {
                        _ieAddressesSet.Add(ga);

                        if (!ieAddresses.ContainsKey(suffix))
                        {
                            ieAddresses[suffix] = new List<XElement>();
                        }
                        ieAddresses[suffix].Add(ga);
                    }
                }
                else if (name.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase))
                {
                    var suffix = name.Substring(3);
                    if (!cmdAddresses.ContainsKey(suffix))
                    {
                        cmdAddresses[suffix] = new List<XElement>();
                    }
                    cmdAddresses[suffix].Add(ga);
                }
            }
        }

        

        // Maintenant, pour chaque adresse "Cmd", on associe les adresses "Ie" correspondantes
        foreach (var cmdEntry in cmdAddresses)
        {
            var suffix = cmdEntry.Key;
            var cmds = cmdEntry.Value;

            foreach (var cmd in cmds)
            {
                var address = cmd.Attribute("Address")?.Value;
                var dpts = cmd.Attribute("DPTs")?.Value;
                if (address != null)
                {
                    if (!addedCmdAddresses.ContainsKey(suffix))
                    {
                        addedCmdAddresses[suffix] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    // Vérifier si la combinaison suffixe/adresse a déjà été ajoutée
                    if (!addedCmdAddresses[suffix].Contains(address))
                    {
                        addedCmdAddresses[suffix].Add(address); // Marquer cette combinaison comme ajoutée

                        // Créez une clé unique pour cette commande
                        var cmdKey = $"{suffix}_{address}_{dpts}";

                        

                        // Ajouter la commande si elle n'a pas encore été ajoutée
                        if (!addedCommandKeys.Contains(cmdKey))
                        {
                            //poursavoir ou on en est
                            int compte = 0;

                            if (ieAddresses.ContainsKey(suffix))
                            {
                                groupAddressProcessor.AddToGroupedAddresses(_groupedAddresses, cmd,
                                $"{suffix}_{address}_{dpts}"); // Ajouter l'adresse "Cmd"
                                                               // Si des adresses "Ie" avec le même suffixe existent, on les associe à l'adresse "Cmd"
                                foreach (var ieGa in ieAddresses[suffix])
                                {
                                    groupAddressProcessor.AddToGroupedAddresses(_groupedAddresses, ieGa,
                                    $"{suffix}_{address}_{dpts}"); // Ajouter les adresses "Ie"
                                    compte += 1;
                                }
                            }
                            else
                            {
                                // Si on ne trouve pas, essayer avec un suffixe modifié
                                var modifiedSuffix = GetModifiedSuffix(suffix);


                                // Vérifier les adresses "Ie" en supprimant le texte entre le deuxième et le troisième tiret
                                foreach (var ieEntry in ieAddresses)
                                {
                                    var ieKey = ieEntry.Key;
                                    var ieList = ieEntry.Value;

                                    foreach (var ie in ieList)
                                    {
                                        var ieName = ie.Attribute("Name")?.Value;
                                        if (ieName != null)
                                        {
                                            var cleanedName = RemoveTextBetweenSecondAndThirdUnderscore(ieName);

                                            if (cleanedName.Equals(modifiedSuffix, StringComparison.OrdinalIgnoreCase))
                                            {
                                                // Ajouter la commande si elle n'a pas encore été ajoutée
                                                if (!addedCommandKeys.Contains(cmdKey))
                                                {
                                                    groupAddressProcessor.AddToGroupedAddresses(_groupedAddresses, cmd, cmdKey);
                                                    addedCommandKeys.Add(cmdKey); // Ajouter la clé au HashSet
                                                }
                                                foreach (var ieGa in ieAddresses[ieKey])
                                                {
                                                    groupAddressProcessor.AddToGroupedAddresses(_groupedAddresses, ieGa,
                                                        $"{suffix}_{address}_{dpts}"); // Ajouter les adresses "Ie"
                                                    compte += 1;
                                                }



                                            }

                                        }
                                    }


                                }

                                // Si toujours rien, ajouter uniquement la commande "Cmd"
                                if (compte == 0)
                                {
                                    groupAddressProcessor.AddToGroupedAddresses(_groupedAddresses, cmd, $"{suffix}_{address}_{dpts}");
                                }

                            }
                            
                        }
                    }
                }
            }
        }

        groupAddressMerger.MergeSingleElementGroups(_groupedAddresses, _ieAddressesSet);
        groupAddressMerger.GetElementsBySimilarity("_VoletRoulant_Position_MaqKnxC_MaisonDupre_RezDeChaussee_Tgbt", _ieAddressesSet);
    }
    
    // Méthode pour modifier le suffixe en retirant la partie après le deuxième underscore mais en gardant ce qui est entre le premier et le deuxième et après le troisième
    private string GetModifiedSuffix(string suffix)
    {
        // Trouver le premier, le deuxième et le troisième underscores dans le suffixe
        int firstUnderscoreIndex = suffix.IndexOf('_');
        int secondUnderscoreIndex = suffix.IndexOf('_', firstUnderscoreIndex + 1);
        int thirdUnderscoreIndex = suffix.IndexOf('_', secondUnderscoreIndex + 1);

        if (firstUnderscoreIndex > -1 && secondUnderscoreIndex > -1 && thirdUnderscoreIndex > -1)
        {
            // Conserver la partie entre le premier et le deuxième underscore et après le troisième
            return suffix.Substring(0, secondUnderscoreIndex + 1) + suffix.Substring(thirdUnderscoreIndex);
        }

        return suffix; // Si pas assez d'underscores, retourner tel quel
    }

    // Méthode pour enlever le texte entre le deuxième et le troisième tiret "_"
    private string RemoveTextBetweenSecondAndThirdUnderscore(string name)
    {
        var parts = name.Split('_');
        if (parts.Length > 3)
        {
            return $"_{parts[1]}__{parts[3]}_{string.Join("_", parts.Skip(4))}";
        }
        return name;
    }

    


    /// <summary>
    /// Determines the level structure of group addresses in an XML document to check for overlaps.
    /// 
    /// This method examines an XML document containing group address ranges and specific group addresses.
    /// It helps in identifying whether the group addresses are organized into 2 levels or 3 levels by detecting if there are any overlapping addresses.
    /// 
    /// If the addresses are detected to overlap, the method returns the value 3.
    /// If no overlaps are found, the method returns the value 2.
    /// 
    /// <param name="doc">The XML document (XDocument) containing the group address ranges and specific group addresses.</param>
    /// <returns>An integer indicating the overlap status: 3 for detected overlap, 2 for no overlap.</returns>
    /// </summary>
    public int DetermineGroupAddressStructure(XDocument doc)
    {
        // Ensemble pour vérifier les chevauchements d'adresses
        HashSet<int> allAddresses = new HashSet<int>();

        // Parcourir chaque GroupRange
        foreach (var groupRange in doc.Descendants(GlobalKnxNamespace + "GroupRange"))
        {
            // Parcourir chaque GroupAddress du GroupRange
            foreach (var groupAddress in groupRange.Descendants(GlobalKnxNamespace + "GroupAddress"))
            {
                int address = int.Parse(groupAddress.Attribute("Address")!.Value);

                // Si l'adresse est déjà dans l'ensemble, il y a chevauchement
                if (!allAddresses.Add(address))
                {
                    // Retourne 3 si un chevauchement est détecté
                    return 3;
                }
            }
        }

        // Si aucun chevauchement n'est trouvé, retourne 2
        return 2;
    }

}