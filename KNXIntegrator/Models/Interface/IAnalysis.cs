using Knx.Falcon;

namespace KNXIntegrator.Models;

public interface IAnalysis
{
    public List<(GroupAddress,GroupValue,GroupAddress,GroupValue?,bool?)> GetRecords(GroupAddress cmdAddr,GroupAddress stateAddr,int cmdSizeInBit);
    
    public bool Check(GroupValue cmdValue,GroupValue stateValue);
    
}