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
        public async Task<MsResult<List<RFIDDevice>>> GetRFIDDevices()
        {
            return await HttpProxy.GetMessage<List<RFIDDevice>>("/api/rfiddevice/get");
        }

        public async Task<MsResult<int>> AddRFIDDevice(RFIDDevice rfidDevice)
        {
            return await HttpProxy.PostMessage<int>("/api/rfiddevice/add", rfidDevice);
        }

        public async Task<MsResult<bool>> RemoveRFIDDevices(List<int> ids)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/rfiddevice/remove", new
            {
                IDS = string.Join(",", ids)
            });
        }
    }
}
