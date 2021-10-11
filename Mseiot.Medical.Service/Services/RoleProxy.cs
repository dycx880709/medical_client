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
        public async Task<MsResult<int>> AddRole(Role role)
        {
            return await HttpProxy.PutMessage<int>("/api/role/add", role);
        }

        public async Task<MsResult<bool>> RemoveRole(int roleId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/role/remove", new KeyValuePair<string, string>("roleId", roleId.ToString()));
        }

        public async Task<MsResult<bool>> ModifyRole(Role role)
        {
            return await HttpProxy.PostMessage<bool>("/api/role/modify", role);
        }

        public async Task<MsResult<List<Role>>> GetRoles()
        {
            return await HttpProxy.GetMessage<List<Role>>("/api/role/gets", null);
        }
    }
}
