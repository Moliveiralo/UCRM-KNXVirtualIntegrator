using Knx.Falcon;
using Knx.Falcon.ApplicationData.DatapointTypes;

namespace KNXIntegrator.Models;

public class Transfer : ITransfer
{

    public List<(GroupAddress addr, GroupValue value)> FrameToSend(GroupAddress cmdAddr, DptSimple cmdDPT, GroupAddress stateAddr, DptSimple stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        ret.Add((cmdAddr, Converter.IntToGroupValue(0,(int)cmdDPT.SizeInBit)));
        ret.Add((cmdAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdDPT.SizeInBit) - 1),(int)cmdDPT.SizeInBit)));
        return ret;
    }

    public List<(GroupAddress addr, GroupValue value)> FrameToReceive(GroupAddress cmdAddr, DptSimple cmdDPT, GroupAddress stateAddr, DptSimple stateDPT)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        ret.Add((stateAddr, Converter.IntToGroupValue(0,(int)cmdDPT.SizeInBit)));
        ret.Add((stateAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)stateDPT.SizeInBit) - 1),(int)stateDPT.SizeInBit)));
        return ret;
    }

    public List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)> Analyze(List<(GroupAddress addr, GroupValue value)> expected, List<(GroupAddress addr, GroupValue value)> actual)
    {
        var ret = new List<(GroupAddress cmdAddr, GroupAddress stateAddr, bool testOK)>();
        for (int i = 0; i < expected.Count; i++)
        {
            //faut changer ça pour avoir une ligne par adresse
            ret.Add((expected[i].addr, actual[i].addr, expected[i].value == actual[i].value));
        }
        return ret;
    }




}