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
        public async Task<MsResult<List<DecontaminateTask>>> GetDecontaminateTasks(List<DecontaminateTaskStatus> decontaminateTaskStatuses)
        {
            return await HttpProxy.GetMessage<List<DecontaminateTask>>("/api/DecontaminateTask/get", new
            {
                decontaminateTaskStatuses = decontaminateTaskStatuses
            });
        }

        public async Task<MsResult<int>> AddDecontaminateTask(DecontaminateTask decontaminateTask)
        {
            return await HttpProxy.PostMessage<int>("/api/DecontaminateTask/add", decontaminateTask);
        }
    }
}
