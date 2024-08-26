using Knx.Falcon;
using Knx.Falcon.ApplicationData.DatapointTypes;

namespace KNXIntegrator.Models;

public class Transfer : ITransfer
{

    public List<(GroupAddress addr, GroupValue value)> FrameToSend(GroupAddress cmdAddr, DptSimple cmdDPT, GroupAddress stateAddr, DptSimple stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        byte bitSize = cmdDPT.GetSizeInBit;
        ret.Add((cmdAddr, new GroupValue(0)));
        //probleme de format de donnée ici
        ret.Add((cmdAddr, new GroupValue(Math.Pow(2,cmdDPT.GetSizeInBit)-1)));
        return ret;
    }

    public List<(GroupAddress addr, GroupValue value)> FrameToReceive(GroupAddress cmdAddr, DptSimple cmdDPT, GroupAddress stateAddr, DptSimple stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        double val_max = cmdDPT.NumericInfo.MaxValue;
        ret.Add((stateAddr, new GroupValue(0)));
        ret.Add((stateAddr, new GroupValue(val_max)));
        return ret;
    }

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(List<(GroupAddress addr, GroupValue value)> expected,List<(GroupAddress addr, GroupValue value)> actual){
        var ret = new List<(GrouptAddress cmdAddr, GroupAddress stateAddr, bool testOK)>();
        for(int i;i<expected.Count;i++){
            //faut changer ça pour avoir une ligne par adresse
            ret.Add((expected[i].addr,actual[i].addr,expected[i].value == actual[i].value));
        }
        return ret;
    }




}