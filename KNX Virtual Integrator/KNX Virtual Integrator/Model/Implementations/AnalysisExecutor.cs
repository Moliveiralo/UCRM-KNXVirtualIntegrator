using System.Xml.Linq;
using KNX_Virtual_Integrator.Model.Interfaces;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Implementations;

public class AnalysisExecutor : IAnalysisExecutor
{
    private IGroupCommunication _comm;
    private IGroupAddressManager _addrManager;
    private Dictionary<string, List<XElement>> _grpAddr;
    private IAnalysis _analyzer;

    public AnalysisExecutor(IGroupCommunication comm, IGroupAddressManager addrManager, IAnalysis analyzer)
    {
        _comm = comm;
        _addrManager = addrManager;
        _grpAddr = addrManager.GetGroupedAddrDict();
        _analyzer = analyzer;
    }

    public async Task<List<Analysis.RecordEntry>> RunAndGetResults()
    {
        var testTable = new List<Analysis.RecordEntry>();
        foreach (var kvp in _grpAddr)
        {
            List<Analysis.RecordEntry> records = _analyzer.GetRecords(
                new GroupAddress(kvp.Value[0].Attribute("Address").Value),
                new GroupAddress(kvp.Value[1].Attribute("Address").Value), 1);
            for (int i = 0; i < records.Count; i++)
            {
                _comm.GroupValueWriteAsync(records[i].CmdAddr, records[i].CmdVal);
                records[i].StateVal = await _comm.MaGroupValueReadAsync(records[i].StateAddr);
                records[i].TestOK = _analyzer.Check(records[i].CmdVal, records[i].StateVal);
            }

            testTable.AddRange(records);
            
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