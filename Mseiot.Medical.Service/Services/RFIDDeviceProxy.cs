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
        public async Task<MsResult<List<RFIDDevice>>> GetRFIDDevices(List<DecontaminateTaskStatus> decontaminateTaskStatuses)
        {
            return await HttpProxy.GetMessage<List<RFIDDevice>>("/api/rfiddevice/get");
        }

        public async Task<MsResult<int>> AddRFIDDevice(RFIDDevice rfidDevice)
        {
            return await HttpProxy.PostMessage<int>("/api/rfiddevice/add", rfidDevice);
        }
    }
}
