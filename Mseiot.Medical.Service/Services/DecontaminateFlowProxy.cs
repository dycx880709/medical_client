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
        #region 流程

        public async Task<MsResult<List<DecontaminateFlow>>> GetDecontaminateFlows()
        {
            return await HttpProxy.GetMessage<List<DecontaminateFlow>>("/api/DecontaminateFlow/get");
        }

        public async Task<MsResult<int>> AddDecontaminateFlow(DecontaminateFlow decontaminateFlow)
        {
            return await HttpProxy.PostMessage<int>("/api/DecontaminateFlow/add", decontaminateFlow);
        }

        public async Task<MsResult<bool>> ModifyDecontaminateFlow(DecontaminateFlow decontaminateFlow)
        {
            return await HttpProxy.PutMessage<bool>("/api/DecontaminateFlow/modify", decontaminateFlow);
        }

        public async Task<MsResult<bool>> RemoveDecontaminateFlows(List<int> decontaminateFlowIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/DecontaminateFlow/remove", new { ids = string.Join(",", decontaminateFlowIDs) });
        }

        #endregion

        #region 流程步骤

        public async Task<MsResult<List<DecontaminateFlowStep>>> GetDecontaminateFlowSteps(int decontaminateFlowID)
        {
            return await HttpProxy.GetMessage<List<DecontaminateFlowStep>>("/api/DecontaminateFlow/GetDecontaminateFlowSteps", new { decontaminateFlowID = decontaminateFlowID });
        }

        public async Task<MsResult<int>> AddDecontaminateFlowStep(DecontaminateFlowStep decontaminateFlowStep)
        {
            return await HttpProxy.PostMessage<int>("/api/DecontaminateFlow/AddDecontaminateFlowStep", decontaminateFlowStep);
        }

        public async Task<MsResult<bool>> ModifyDecontaminateFlowStep(DecontaminateFlowStep decontaminateFlowStep)
        {
            return await HttpProxy.PutMessage<bool>("/api/DecontaminateFlow/ModifyDecontaminateFlowStep", decontaminateFlowStep);
        }

        public async Task<MsResult<bool>> RemoveDecontaminateFlowSteps(List<int> decontaminateFlowStepIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/DecontaminateFlow/RemoveDecontaminateFlowStep", new { ids = string.Join(",", decontaminateFlowStepIDs) });
        }

        #endregion
    }
}
