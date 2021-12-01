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
        public async Task<MsResult<List<Endoscope>>> GetEndoscopes()
        {
            return await HttpProxy.GetMessage<List<Endoscope>>("/api/Endoscope/gets");
        }

        public async Task<MsResult<Endoscope>> GetEndoscopeById(int endoscopeId)
        {
            return await HttpProxy.GetMessage<Endoscope>("/api/Endoscope/get", new { EndoscopeId = endoscopeId });
        }

        public async Task<MsResult<int>> AddEndoscope(Endoscope endoscope)
        {
            return await HttpProxy.PostMessage<int>("/api/Endoscope/add", endoscope);
        }

        public async Task<MsResult<bool>> RemoveEndoscopes(List<int> endoscopeIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/Endoscope/remove", new { ids = string.Join(",", endoscopeIDs) });
        }

        public async Task<MsResult<bool>> ModifyEndoscope(Endoscope endoscope)
        {
            return await HttpProxy.PostMessage<bool>("/api/Endoscope/modify", endoscope);
        }
    }
}
