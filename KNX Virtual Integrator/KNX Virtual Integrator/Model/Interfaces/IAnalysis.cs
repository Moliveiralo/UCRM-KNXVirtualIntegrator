using KNX_Virtual_Integrator.Model.Implementations;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Interfaces;

public interface IAnalysis
{
    //call this method to get a Record Entry. You get a RecordEntry, then change the null attributes with value you got
    public List<Analysis.RecordEntry> GetRecords(GroupAddress cmdAddr,GroupAddress stateAddr,int cmdSizeInBit);
    
    //very simple checker for now, check if value are equal. 
    public bool Check(GroupValue cmdValue,GroupValue stateValue);
    

}