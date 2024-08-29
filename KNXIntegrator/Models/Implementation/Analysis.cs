using Knx.Falcon;

namespace KNXIntegrator.Models;

public class Analysis:IAnalysis
{
    public List<(GroupAddress, GroupValue, GroupAddress, GroupValue?, bool?)> GetRecords(GroupAddress cmdAddr,
        GroupAddress stateAddr,int cmdSizeInBit)
    {
        var ret = new List<(GroupAddress, GroupValue, GroupAddress, GroupValue?,bool?)>();
        ret.Add((cmdAddr, Converter.IntToGroupValue(0,cmdSizeInBit),stateAddr,null,null));
        ret.Add((cmdAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdSizeInBit) - 1),cmdSizeInBit),stateAddr,null,null));
        return ret;
        
    }

    public bool Check(GroupValue cmdValue, GroupValue stateValue)
    {
        return Equals(cmdValue,stateValue);
    }
    
}