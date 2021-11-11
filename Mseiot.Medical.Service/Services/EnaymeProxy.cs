using Ms.Libs.Models;
using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Services
{
    public partial class SocketProxy
    {
        public async Task<MsResult<List<Enzyme>>> GetEnzymes()
        {
            return await HttpProxy.GetMessage<List<Enzyme>>("/api/Enzyme/GetEnzymes");
        }

        public async Task<MsResult<int>> AddEnzyme(Enzyme enzyme)
        {
            return await HttpProxy.PostMessage<int>("/api/Enzyme/AddEnzyme", enzyme);
        }

        public async Task<MsResult<bool>> RemoveEnzymes(List<int> enzymeIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/Enzyme/RemoveEnzymes", new { ids = string.Join(",", enzymeIDs) });
        }

        public async Task<MsResult<bool>> ModifyEnzyme(Enzyme enzyme)
        {
            return await HttpProxy.PostMessage<bool>("/api/Enzyme/ModifyEnzyme", enzyme);
        }

        public async Task<MsResult<bool>> ChangeModifyEnzymeSelected(Enzyme enzyme)
        {
            return await HttpProxy.PostMessage<bool>("/api/Enzyme/ChangeModifyEnzymeSelected", enzyme);
        }

        
    }
}
