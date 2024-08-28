using Knx.Falcon;
using Knx.Falcon.ApplicationData.DatapointTypes;

namespace KNXIntegrator.Models;

public class Transfer : ITransfer
{

    public List<(GroupAddress addr, GroupValue value)> FrameToSend(GroupAddress cmdAddr,int cmdSizeInBit,GroupAddress stateAddr,int stateSizeInBit)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        ret.Add((cmdAddr, Converter.IntToGroupValue(0,cmdSizeInBit)));
        ret.Add((cmdAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdSizeInBit) - 1),cmdSizeInBit)));
        return ret;
    }

    public List<(GroupAddress addr, GroupValue value)> FrameToReceive(GroupAddress cmdAddr,int cmdSizeInBit,GroupAddress stateAddr,int stateSizeInBit)
    {
        var ret = new List<(GroupAddress addr, GroupValue value)>();
        ret.Add((stateAddr, Converter.IntToGroupValue(0,stateSizeInBit)));
        ret.Add((stateAddr, Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)stateSizeInBit) - 1),stateSizeInBit)));
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