using KNXIntegrator.Falcon.Sdk;

namespace KNXIntegrator.Models;

public class Transfer : ITransfer
{

    public List<(GroupAddress addr, GroupValue value)> FrameToSend(GroupAddress cmdAddr, DptSimpleGeneric cmdDPT, GroupAddress stateAddr, DptSimpleGeneric stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        int val_max = cmdDPT.NumericInfo.MaxValue;
        ret.Add((cmdAddr, new GroupValue(0)));
        ret.Add((cmdAddr, new GroupValue(val_max)));
        return ret;
    }

    public List<(GroupAddress addr, GroupValue value)> FrameToReceive(GroupAddress cmdAddr, DptSimpleGeneric cmdDPT, GroupAddress stateAddr, DptSimpleGeneric stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        int val_max = cmdDPT.NumericInfo.MaxValue;
        ret.Add((stateAddr, new GroupValue(0)));
        ret.Add((stateAddr, new GroupValue(val_max)));
        return ret;
    }

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(GroupAddress cmdAddr, DptSimpleGeneric cmdDPT, GroupAddress stateAddr, DptSimpleGeneric stateDPT,List<(GroupAddress addr, GroupValue value)> actual){
        var ret = new List<(GrouptAddress cmdAddr, GroupAddress stateAddr, bool testOK)>();
        for(int i;i<expected.Count;i++){
            //faut changer ça pour avoir une ligne par adresse
            ret.Add((expected[i].addr,actual[i].addr,expected[i].value == actual[i].value));
        }
        return ret;
    }




}