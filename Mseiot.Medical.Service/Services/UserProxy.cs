using Ms.Libs.Models;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Services
{
    public partial class SocketProxy
    {
        public async Task<MsResult<string>> Login(string userId)
        {
            return await TcpProxy.SendMessage<string>(Command.Module_System, Command.System_Login, userId);
        }
        public async Task<MsResult<User>> Login(string loginName, string loginPwd)
        {
            var result = await HttpProxy.PostMessage<User>("/api/user/login", new
            {
                LoginName = loginName,
                LoginPwd = loginPwd
            });
            if (result.IsSuccess)
                HttpProxy.SetToken(result.Content.Token);
            return result;
        }

        public async Task<MsResult<string>> AddUser(User user)
        {
            return await HttpProxy.PutMessage<string>("/api/user/adduser", user);
        }

        public async Task<MsResult<bool>> RemoveUser(string userId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/user/removeuser", new KeyValuePair<string, string>("userid", userId));
        }

        public async Task<MsResult<bool>> ModifyUser(User user)
        {
            return await HttpProxy.PostMessage<bool>("/api/user/modifyuser", user);
        }

        public async Task<MsResult<UserResult>> GetUsers(
            int index,
            int count,
            string name,
            string sortType = "",
            int[] roleIds = null,
            int[] organizationids = null
        )
        {
            var dir = new Dictionary<string, string>();
            dir.Add("index", index.ToString());
            dir.Add("count", count.ToString());
            if (!string.IsNullOrEmpty(name))
                dir.Add("name", name);
            if (roleIds != null)
                dir.Add("roles", string.Join(",", roleIds.Select(t => (int)t)));
            if (organizationids != null)
                dir.Add("organizationids", string.Join(",", organizationids));
            return await HttpProxy.GetMessage<UserResult>("/api/user/getusers", dir);
        }

        public async Task<MsResult<User>> GetUserById(string userId)
        {
            return await HttpProxy.GetMessage<User>("/api/user/getuserbyid", new KeyValuePair<string, string>("userid", userId));
        }

        public async Task<MsResult<string>> ImportUser(User user)
        {
            return await HttpProxy.PostMessage<string>("/api/user/importuser", user);
        }

        public async Task<MsResult<string>> ModifyPwd(string userId, string oldPwd, string newPwd)
        {
            return await HttpProxy.PostMessage<string>("/api/user/modifypwd", new
            {
                UserID = userId,
                OldPwd = oldPwd,
                NewPwd = newPwd
            });
        }
    }
}
