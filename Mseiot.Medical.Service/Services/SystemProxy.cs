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
        public async Task<MsResult<bool>> UpdateSystemSetting(SystemSetting setting)
        {
            return await HttpProxy.PutMessage<bool>("/api/system/update", setting);
        }

        public async Task<MsResult<SystemSetting>> GetSystemSetting()
        {
            return await HttpProxy.PutMessage<SystemSetting>("/api/system/get", null);
        }
    }
}
