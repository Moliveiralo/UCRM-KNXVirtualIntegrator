using KNXIntegrator.Falcon.Sdk;
namespace KNXIntegrator.Models;

public interface ITransfer{

    public List<(int addr,GroupValue value)> GetSendFrame(int addr,FunctionalModel model);

    public List<(int addr,GroupValue value)> GetExpectedFrame(int addr,FunctionalModel model);
    

}