using Knx.Falcon.DataSecurity;

namespace KNXIntegrator.Models;

public interface IAnalysisExecutor
{
    
    
    public Task<List<Analysis.RecordEntry>> RunAndGetResults();

    public Task<List<string>> RunAndGetResultsInString();

}