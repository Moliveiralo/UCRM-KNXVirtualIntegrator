using KNX_Virtual_Integrator.Model.Interfaces;
using KNX_Virtual_Integrator.Model.Entities;
using KNX_Virtual_Integrator.Model.Tools;
using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Implementations;

public class Analysis : IAnalysis
{
    public record RecordEntry
    {
        public GroupAddress CmdAddr { get; init; }
        public GroupValue CmdVal { get; init; }
        public GroupAddress StateAddr { get; init; }
        public GroupValue? StateVal { get; set; }
        public bool? TestOK { get; set; }

        public override string ToString()
        {
            return ("CmdAddr = " + CmdAddr.ToString() + "\nCmdVal = " + CmdVal.ToString() + "\nStateAddr = " +
                    StateAddr.ToString() + "\nStateVal = " + StateVal.ToString() + "\nTestOK = " + TestOK.ToString() +
                    "\n\n");
        }
    }

    public List<RecordEntry> GetRecords(GroupAddress cmdAddr,
        GroupAddress stateAddr, int cmdSizeInBit)
    {
        var ret = new List<RecordEntry>();
        ret.Add(new RecordEntry
        {
            CmdAddr = cmdAddr,
            CmdVal = Converter.IntToGroupValue(0, cmdSizeInBit), //valeur 0
            StateAddr = stateAddr,
            StateVal = null,
            TestOK = null
        });
        ret.Add(new RecordEntry
        {
            CmdAddr = cmdAddr,
            CmdVal = Converter.IntToGroupValue(((int)Math.Pow(2.0, (double)cmdSizeInBit) - 1), cmdSizeInBit), //valeur max
            StateAddr = stateAddr,
            StateVal = null,
            TestOK = null
        });
        return ret;
    }

    //Le check est pas bon = pas la bonne logique
    public bool Check(GroupValue cmdValue, GroupValue stateValue)
    {
        return Equals(cmdValue, stateValue);
    }
}