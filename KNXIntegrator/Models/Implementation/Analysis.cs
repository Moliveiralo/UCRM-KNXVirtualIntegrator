using Knx.Falcon;

namespace KNXIntegrator.Models;

public class Analysis:IAnalysis
{
    
    public record RecordEntry(GroupAddress CmdAddr, GroupValue CmdVal, GroupAddress StateAddr, GroupValue? StateVal, bool? TestOK);
    public List<RecordEntry> GetRecords(GroupAddress cmdAddr,
        GroupAddress stateAddr,int cmdSizeInBit)
    {
        var ret = new List<RecordEntry>();
        ret.Add(new RecordEntry(cmdAddr, Converter.IntToGroupValue(0,cmdSizeInBit),stateAddr,null,null));
        ret.Add(new RecordEntry(cmdAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdSizeInBit) - 1),cmdSizeInBit),stateAddr,null,null));
        return ret;
        
    }

    public bool Check(GroupValue cmdValue, GroupValue stateValue)
    {
        return Equals(cmdValue,stateValue);
    }
    
}