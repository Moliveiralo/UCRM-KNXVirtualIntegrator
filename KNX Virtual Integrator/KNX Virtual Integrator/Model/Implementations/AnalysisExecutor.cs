using System.Xml.Linq;
using KNX_Virtual_Integrator.Model.Interfaces;
using Knx.Falcon;
using Knx.Falcon.ApplicationData.MasterData;

using KNX_Virtual_Integrator.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace KNX_Virtual_Integrator.Model.Implementations;

public class AnalysisExecutor : IAnalysisExecutor
{
    private IGroupCommunication _comm;
    //private IGroupAddressManager _addrManager;
    private IAddressModelLinker _addressModelLinker;
    //private Dictionary<string, List<XElement>> _grpAddr;
    private Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)> _grpAddr;
    private IAnalysis _analyzer;

    public AnalysisExecutor(IGroupCommunication comm, IAddressModelLinker addressModelLinker/*IGroupAddressManager addrManager*/, IAnalysis analyzer)
    {
        _comm = comm;
        //_addrManager = addrManager;
        _addressModelLinker = addressModelLinker;
        //_grpAddr = addrManager.GetGroupedAddrDict();
        //_grpAddr = await addressModelLinker.CreateInformationsGlobAsync();
        _analyzer = analyzer;
    }

    public async Task<List<Analysis.RecordEntry>> RunAndGetResults()
    {
        //prendre les infos à envoyer
        var _info = await _addressModelLinker.CreateInformationsGlobAsync();
        var testTable = new List<Analysis.RecordEntry>();
        //pour chaque info : faire un envoi et une lecture
        foreach (var kvp in _info)
        {
            /*       if (kvp.Value.Count < 2)
                   {
                       // Log or handle the error
                       continue; // Skip this entry
                   }
           List<Analysis.RecordEntry> records = _analyzer.GetRecords(
               new GroupAddress(kvp.Value[0].Attribute("Address").Value),
               new GroupAddress(kvp.Value[1].Attribute("Address").Value), 1);
           for (int i = 0; i < records.Count; i++)
           {
               _comm.GroupValueWriteAsync(records[i].CmdAddr, records[i].CmdVal);
               records[i].StateVal = await _comm.MaGroupValueReadAsync(records[i].StateAddr);
               records[i].TestOK = _analyzer.Check(records[i].CmdVal, records[i].StateVal);
           }

           testTable.AddRange(records);*/

        }
        return testTable;
    }
    
    public async Task<List<string>> RunAndGetResultsInStringList()
    {
        List<Analysis.RecordEntry> testTable = await RunAndGetResults();
        var testTableInString = new List<string>();
        foreach (var record in testTable)
        {
            testTableInString.Add(record.ToString());
        }
        
        return testTableInString;
    }

    public async Task<string> RunAndGetResultsInString()
    {
        List<string> list = await RunAndGetResultsInStringList();
        string result = "";
        foreach (string str in list)
        {
            result = (result+str);
        }

        return result;
    }
}