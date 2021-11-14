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
        public async Task<MsResult<List<SQLInfo>>> GetDBRecords()
        {
            return await HttpProxy.GetMessage<List<SQLInfo>>("/api/backup/GetDBRecords");
        }


        public async Task<MsResult<bool>> RemoveDBRecords(List<string> paths)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/backup/RemoveDBRecords", new { paths = string.Join(",", paths) });
        }


        
    }
}
