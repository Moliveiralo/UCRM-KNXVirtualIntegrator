﻿using System.Xml.Linq;
using KNX_Virtual_Integrator.Model.Interfaces;

namespace KNX_Virtual_Integrator.Model.Implementations;

public class GroupAddressManager(Logger logger, ProjectFileManager projectFileManager, FileLoader loader, NamespaceResolver namespaceResolver, GroupAddressProcessor groupAddressProcessor, GroupAddressMerger groupAddressMerger) : IGroupAddressManager
{
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Dictionary where each Cmd addresses is grouped with the corresponding Ie adresses
    /// </summary>
    public readonly Dictionary<string, List<XElement>> GroupedAddresses = new ();
    
    /// <summary>
    /// List of all the Ie addresses
    /// </summary>
    public readonly List<XElement> IeAddressesSet = new();

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
        
        namespaceResolver.SetNamespaceFromXml(filePath);

        if (namespaceResolver.GlobalKnxNamespace == null) return;

        if (filePath == manager.ZeroXmlPath)
        {
            ProcessZeroXmlFile(groupAddressFile);
        }
        else
        {
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
        // Initialiser le dictionnaire pour les adresses qui commencent par "Ie" avec un HashSet pour éviter les doublons
        var ieAddresses = new HashSet<string>();
        
        IeAddressesSet.Clear();
        GroupedAddresses.Clear();

        var groupAddressStructure = DetermineGroupAddressStructure(groupAddressFile);
        
        // Étape 1 : Extraire les références des appareils
        var deviceRefs = groupAddressFile.Descendants(namespaceResolver.GlobalKnxNamespace! + "DeviceInstance")
            .Select(di => (
                Id: di.Attribute("Id")?.Value,
                Links: di.Descendants(namespaceResolver.GlobalKnxNamespace! + "ComObjectInstanceRef")
                    .Where(cir => cir.Attribute("Links") != null)
                    .SelectMany(cir => cir.Attribute("Links")?.Value.Split(' ') ?? Array.Empty<string>())
                    .ToHashSet()
            ))
            .ToList();

        var groupAddresses = groupAddressFile.Descendants(namespaceResolver.GlobalKnxNamespace! + "GroupAddress").ToList();
        var tempGroupedAddresses = new Dictionary<(string CommonName, string DeviceId, string CmdAddress), HashSet<string>>();

        // Étape 2 : Regrouper les adresses par nom commun et ID de l'appareil
        foreach (var ga in groupAddresses)
        {
            var id = ga.Attribute("Id")?.Value;
            var name = ga.Attribute("Name")?.Value;
            // Convert the address to the x/x/x format (depending on the groupAddressStructure)
            var address = groupAddressProcessor.DecodeAddress(ga.Attribute("Address")?.Value ?? string.Empty, groupAddressStructure); 

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

                    if (!tempGroupedAddresses.ContainsKey(key!))
                    {
                        tempGroupedAddresses[key!] = new HashSet<string>();
                    }

                    tempGroupedAddresses[key!].Add(id);
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
                        IeAddressesSet.Add(ga);
                    }
                }
            }
        }

        // Étape 3 : Regrouper les adresses "Cmd" et "Ie" sous la même clé, en tenant compte du DeviceId
        foreach (var entry in tempGroupedAddresses)
        {
            var commonName = $"{entry.Key.CommonName}_{entry.Key.CmdAddress}";
            var gaIds = entry.Value;

            // Chercher l'entrée existante basée sur le commonName et le DeviceId
            var existingEntry = GroupedAddresses.FirstOrDefault(g =>
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
                GroupedAddresses[commonName] = gaIds.Select(id => groupAddresses.First(x => x.Attribute("Id")?.Value == id)).ToList();
            }
        }

        // Étape 4 : Finaliser les regroupements sous les adresses "Cmd"
        foreach (var entry in GroupedAddresses)
        {
            var cmdAddress = entry.Value.FirstOrDefault(x => (bool)x.Attribute("Name")?.Value.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase));

            if (cmdAddress != null)
            {
                var commonName = entry.Key;
                GroupedAddresses[commonName] = new List<XElement> { cmdAddress };

                // Ajouter toutes les adresses "Ie" correspondantes sous le même nom commun
                GroupedAddresses[commonName].AddRange(entry.Value.Where(x => (bool)x.Attribute("Name")?.Value.StartsWith("Ie", StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                // Si aucune adresse "Cmd" n'est trouvée, ajouter le groupe tel quel
                GroupedAddresses[entry.Key] = entry.Value;
            }
        }
       
        groupAddressMerger.MergeSingleElementGroups(GroupedAddresses, IeAddressesSet);
        
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
        IeAddressesSet.Clear();
        GroupedAddresses.Clear();
        var groupAddresses = groupAddressFile.Descendants(namespaceResolver.GlobalKnxNamespace! + "GroupAddress").ToList();
    
        var ieAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
        var cmdAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
        var addedCmdAddresses = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var ga in groupAddresses)
        {
            var name = ga.Attribute("Name")?.Value;
            var address = ga.Attribute("Address")?.Value;
            if (name != null && address != null)
            {
                if (name.StartsWith("Ie", StringComparison.OrdinalIgnoreCase))
                {
                    var suffix = name.Substring(2);
                    // Vérifier si l'adresse est déjà présente dans la liste ieAddressesSet
                    if (IeAddressesSet.All(x => x.Attribute("Address")?.Value != address))
                    {
                        IeAddressesSet.Add(ga);

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

                        if (ieAddresses.ContainsKey(suffix))
                        {
                            groupAddressProcessor.AddToGroupedAddresses(GroupedAddresses, cmd,
                                $"{suffix}_{address}"); // Ajouter l'adresse "Cmd"
                            // Si des adresses "Ie" avec le même suffixe existent, on les associe à l'adresse "Cmd"
                            foreach (var ieGa in ieAddresses[suffix])
                            {
                                groupAddressProcessor.AddToGroupedAddresses(GroupedAddresses, ieGa,
                                    $"{suffix}_{address}"); // Ajouter les adresses "Ie"
                            }
                        }
                        else
                        {
                            // Si aucune adresse "Ie" ne correspond, on ajoute uniquement l'adresse "Cmd"
                            groupAddressProcessor.AddToGroupedAddresses(GroupedAddresses, cmd, $"{suffix}_{address}");
                        }
                    }
                }
            }
        }

        groupAddressMerger.MergeSingleElementGroups(GroupedAddresses, IeAddressesSet);
        groupAddressMerger.GetElementsBySimilarity("_VoletRoulant_Position_MaqKnxC_MaisonDupre_RezDeChaussee_Tgbt", IeAddressesSet);
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
        var allAddresses = new HashSet<int>();

        // Parcourir chaque GroupRange
        foreach (var groupRange in doc.Descendants(namespaceResolver.GlobalKnxNamespace! + "GroupRange"))
        {
            // Parcourir chaque GroupAddress du GroupRange
            foreach (var groupAddress in groupRange.Descendants(namespaceResolver.GlobalKnxNamespace! + "GroupAddress"))
            {
                var address = int.Parse(groupAddress.Attribute("Address")!.Value);

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