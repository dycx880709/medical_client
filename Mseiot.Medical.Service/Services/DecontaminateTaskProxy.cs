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
        public async Task<MsResult<ListResult<DecontaminateTask>>> GetDecontaminateTasks(
            int index,
            int count,
            List<DecontaminateTaskStatus> decontaminateTaskStatuses,
            string searchContent,
            DateTime? startTime,
            DateTime? endTime
        )
        {
            return await HttpProxy.GetMessage<ListResult<DecontaminateTask>>("/api/DecontaminateTask/get", new
            {
                Index = index,
                Count = count,
                DecontaminateTaskStatuses = decontaminateTaskStatuses != null ? string.Join(",", decontaminateTaskStatuses.Select(t => (int)t)) : null,
                SearchContent = searchContent,
                StartTime = startTime == null ? 0: TimeHelper.ToUnixTime((DateTime)startTime),
                EndTime = endTime == null ? TimeHelper.ToUnixTime(DateTime.Now) : TimeHelper.ToUnixTime(endTime.Value),
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
