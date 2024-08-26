using Knx.Falcon;
using Knx.Falcon.ApplicationData.DatapointTypes;

namespace KNXIntegrator.Models;

public interface ITransfer{

    public List<(GroupAddress addr,GroupValue value)> FrameToSend(GroupAddress cmdAddr,DptSimple cmdDPT,GroupAddress stateAddr, DptSimple stateDPT);

    public List<(GroupAddress addr,GroupValue value)> FrameToReceive(GroupAddress cmdAddr,DptSimple cmdDPT,GroupAddress stateAddr, DptSimple stateDPT);

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(List<(GroupAddress addr, GroupValue value)> expected,List<(GroupAddress addr, GroupValue value)> actual);

    

}