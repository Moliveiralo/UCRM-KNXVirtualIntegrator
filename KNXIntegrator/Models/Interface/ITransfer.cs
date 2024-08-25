using KNXIntegrator.Falcon.Sdk;
namespace KNXIntegrator.Models;

public interface ITransfer{

    public List<(GroupAddress addr,GroupValue value)> FrameToSend(GroupAddress cmdAddr,DptSimpleGeneric cmdDPT,GroupAddress stateAddr, DptSimpleGeneric stateDPT);

    public List<(GroupAddress addr,GroupValue value)> FrameToReceive(GroupAddress cmdAddr,DptSimpleGeneric cmdDPT,GroupAddress stateAddr, DptSimpleGeneric stateDPT);

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(List<(GroupAddress addr, GroupValue value)> expected,List<(GroupAddress addr, GroupValue value)> received);

    

}