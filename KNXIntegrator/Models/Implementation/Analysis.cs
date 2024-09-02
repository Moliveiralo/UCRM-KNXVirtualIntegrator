using Knx.Falcon;

namespace KNXIntegrator.Models;

public class Analysis:IAnalysis
{

    public record RecordEntry
    {
        public GroupAddress CmdAddr { get; set;}
        public GroupValue CmdVal { get; set;}
        public GroupAddress StateAddr { get; set;}
        public GroupValue? StateVal { get; set;}
        public bool? TestOK { get; set;}
        
    }
    public List<RecordEntry> GetRecords(GroupAddress cmdAddr,
        GroupAddress stateAddr,int cmdSizeInBit)
    {
        var ret = new List<RecordEntry>();
        ret.Add(new RecordEntry
        {
            CmdAddr = cmdAddr,
            CmdVal = Converter.IntToGroupValue(0, cmdSizeInBit),
            StateAddr = stateAddr,
            StateVal = null,
            TestOK = null
        });
        ret.Add(new RecordEntry
        {
            CmdAddr = cmdAddr,
            CmdVal = Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdSizeInBit) - 1), cmdSizeInBit),
            StateAddr = stateAddr,
            StateVal = null,
            TestOK = null
        });
        return ret;
        
    }

    public bool Check(GroupValue cmdValue, GroupValue stateValue)
    {
        return Equals(cmdValue,stateValue);
    }
    
}