using Ms.Libs.Models;
using Ms.Libs.SysLib;
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
        public async Task<MsResult<List<DecontaminateTask>>> GetDecontaminateTasks(List<DecontaminateTaskStatus> decontaminateTaskStatuses,string searchContent,DateTime? startTime,DateTime? endTime)
        {
            return await HttpProxy.GetMessage<List<DecontaminateTask>>("/api/DecontaminateTask/get", new
            {
                decontaminateTaskStatuses = decontaminateTaskStatuses != null ? string.Join(",", decontaminateTaskStatuses.Select(t => (int)t)) : null,
                SearchContent = searchContent,
                StartTime = startTime == null ?0: TimeHelper.ToUnixTime((DateTime)startTime),
               EndTime = endTime == null ? long.MaxValue : TimeHelper.ToUnixTime((DateTime)endTime),
            }) ;
        }

        public async Task<MsResult<int>> AddDecontaminateTask(DecontaminateTask decontaminateTask)
        {
            return await HttpProxy.PostMessage<int>("/api/DecontaminateTask/add", decontaminateTask);
        }

        public async Task<MsResult<bool>> ChangeDecontaminateTaskStatus(DecontaminateTask decontaminateTask)
        {
            return await HttpProxy.PostMessage<bool>("/api/DecontaminateTask/ChangeDecontaminateTaskStatus", decontaminateTask);
        }
    }
}
