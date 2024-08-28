using Knx.Falcon;
using Knx.Falcon.ApplicationData.DatapointTypes;

namespace KNXIntegrator.Models;

public interface ITransfer{

    public List<(GroupAddress addr,GroupValue value)> FrameToSend(GroupAddress cmdAddr,int cmdFrameSize,GroupAddress stateAddr,int stateFrameSize);

    public List<(GroupAddress addr,GroupValue value)> FrameToReceive(GroupAddress cmdAddr,int cmdFrameSize,GroupAddress stateAddr,int stateFrameSize);

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(List<(GroupAddress addr, GroupValue value)> expected,List<(GroupAddress addr, GroupValue value)> actual);

    

}