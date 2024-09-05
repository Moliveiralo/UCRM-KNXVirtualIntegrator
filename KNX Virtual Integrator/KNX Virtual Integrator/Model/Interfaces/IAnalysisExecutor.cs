using Knx.Falcon.DataSecurity;
using KNX_Virtual_Integrator.Model.Implementations;

namespace KNX_Virtual_Integrator.Model.Interfaces;

public interface IAnalysisExecutor
{
    //DESCRIPTION----------------------------------------------------------------------------------------
    //initialize with constructor AnalysisExecutor(IGroupCommunication comm, IGroupAddressManager addrManager, IAnalysis analyzer)
    //GroupedAddresses of GroupAddressManager should already be filled with data, and connection to bus should be already active.
    //instanciate the class in ModelManager
    //use RunAndGetResultsInString() to run the analysis and get the result in return value. Method called by ViewModel.
    //DEPENDENCY----------------------------------------------------------------------------------------
    //IGroupCommunication should present methods Task GroupValueWriteAsync((GroupAddress addr,GroupValue value) toWrite) and Task<GroupValue> MaGroupValueReadAsync(GroupAddress toRead), and communication to bus should be connected.
    //IGroupAddressManager should present method Dictionary<string, List<XElement>> GetGroupedAddrDict(), and the dictionary GroupedAddresses should already be filled with data
    //IAnalysis should present method List<Analysis.RecordEntry> GetRecords(GroupAddress cmdAddr,GroupAddress stateAddr,int cmdSizeInBit) and bool Check(GroupValue cmdValue,GroupValue stateValue)
    //-------------------------------------------------------------------------------------------------------------
    
    public Task<List<Analysis.RecordEntry>> RunAndGetResults();

    public Task<List<string>> RunAndGetResultsInString();

}