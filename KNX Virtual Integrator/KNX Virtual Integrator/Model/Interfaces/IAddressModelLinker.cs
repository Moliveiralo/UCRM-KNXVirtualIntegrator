using Knx.Falcon.ApplicationData.MasterData;
using Knx.Falcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNX_Virtual_Integrator.Model.Interfaces
{
    public interface IAddressModelLinker
    {
        public Task<Dictionary<string, (GroupAddress CmdAddress, GroupAddress IeAddress, DatapointType Dpt, DatapointSubtype SubDpt, int SizeInBit, GroupValue Ve, GroupValue Va)>> CreateInformationsGlobAsync();

        public Task<List<string>> GetFormattedLinkResultAsync();

        public Task<string> GetFormattedLinkResultInString();
    }
}
