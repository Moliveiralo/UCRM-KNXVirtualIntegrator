// using System.Xml.Linq;
//
// namespace KNXIntegrator.Models;
//
// public class GrpAddrRepository
// {
//     public static XNamespace GlobalKnxNamespace = "http://knx.org/xml/ga-export/01";
//     public readonly Dictionary<string, List<XElement>> GroupedAddresses = new ();
//     public readonly List<XElement> IeAddressesSet = new();
//
//     
//     
//     /// <summary>
//     /// Processes an XML file in the standard format to extract and group addresses.
//     ///
//     /// This method processes group addresses from the XML file, normalizing the names by removing
//     /// specific prefixes ("Ie" or "Cmd") and grouping addresses based on the remaining common names.
//     ///
//     /// <param name="groupAddressFile">The XML document containing group address data in standard format.</param>
//     /// </summary>
//     public void ProcessStandardXmlFile(XDocument groupAddressFile)
//     {
//         GlobalKnxNamespace = "http://knx.org/xml/ga-export/01";
//         IeAddressesSet.Clear();
//         GroupedAddresses.Clear();
//         var groupAddresses = groupAddressFile.Descendants(GlobalKnxNamespace + "GroupAddress").ToList();
//     
//         var ieAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
//         var cmdAddresses = new Dictionary<string, List<XElement>>(StringComparer.OrdinalIgnoreCase);
//         var addedCmdAddresses = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
//         
//         foreach (var ga in groupAddresses)
//         {
//             var name = ga.Attribute("Name")?.Value;
//             var address = ga.Attribute("Address")?.Value;
//             if (name != null && address != null)
//             {
//                 if (name.StartsWith("Ie", StringComparison.OrdinalIgnoreCase))
//                 {
//                     var suffix = name.Substring(2);
//                     // Vérifier si l'adresse est déjà présente dans la liste ieAddressesSet
//                     if (!IeAddressesSet.Any(x => x.Attribute("Address")?.Value == address))
//                     {
//                         IeAddressesSet.Add(ga);
//
//                         if (!ieAddresses.ContainsKey(suffix))
//                         {
//                             ieAddresses[suffix] = new List<XElement>();
//                         }
//                         ieAddresses[suffix].Add(ga);
//                     }
//                 }
//                 else if (name.StartsWith("Cmd", StringComparison.OrdinalIgnoreCase))
//                 {
//                     var suffix = name.Substring(3);
//                     if (!cmdAddresses.ContainsKey(suffix))
//                     {
//                         cmdAddresses[suffix] = new List<XElement>();
//                     }
//                     cmdAddresses[suffix].Add(ga);
//                 }
//             }
//         }
//
//         // Maintenant, pour chaque adresse "Cmd", on associe les adresses "Ie" correspondantes
//         foreach (var cmdEntry in cmdAddresses)
//         {
//             var suffix = cmdEntry.Key;
//             var cmds = cmdEntry.Value;
//
//             foreach (var cmd in cmds)
//             {
//                 var address = cmd.Attribute("Address")?.Value;
//                 if (address != null)
//                 {
//                     if (!addedCmdAddresses.ContainsKey(suffix))
//                     {
//                         addedCmdAddresses[suffix] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//                     }
//
//                     // Vérifier si la combinaison suffixe/adresse a déjà été ajoutée
//                     if (!addedCmdAddresses[suffix].Contains(address))
//                     {
//                         addedCmdAddresses[suffix].Add(address); // Marquer cette combinaison comme ajoutée
//
//                         if (ieAddresses.ContainsKey(suffix))
//                         {
//                             AddToGroupedAddresses(GroupedAddresses, cmd,
//                                 $"{suffix}_{address}"); // Ajouter l'adresse "Cmd"
//                             // Si des adresses "Ie" avec le même suffixe existent, on les associe à l'adresse "Cmd"
//                             foreach (var ieGa in ieAddresses[suffix])
//                             {
//                                 AddToGroupedAddresses(GroupedAddresses, ieGa,
//                                     $"{suffix}_{address}"); // Ajouter les adresses "Ie"
//                             }
//                         }
//                         else
//                         {
//                             // Si aucune adresse "Ie" ne correspond, on ajoute uniquement l'adresse "Cmd"
//                             AddToGroupedAddresses(GroupedAddresses, cmd, $"{suffix}_{address}");
//                         }
//                     }
//                 }
//             }
//         }
//       
//     }
//     
//     /// <summary>
//     /// Adds a group address to the grouped addresses dictionary with a normalized common name.
//     ///
//     /// This method ensures that the group address is added to the list associated with the specified
//     /// common name. If the common name does not already exist in the dictionary, it is created.
//     ///
//     /// <param name="groupedAddresses">The dictionary of grouped addresses where the group address will be added.</param>
//     /// <param name="ga">The group address element to be added.</param>
//     /// <param name="commonName">The common name used for grouping the address.</param>
//     /// </summary>
//     public void AddToGroupedAddresses(
//         Dictionary<string, List<XElement>> groupedAddresses,
//         XElement ga,
//         string commonName)
//     {
//         if (!groupedAddresses.ContainsKey(commonName))
//         { 
//             groupedAddresses[commonName] = new List<XElement>();
//         } 
//         groupedAddresses[commonName].Add(ga);
//     }
//     
// }