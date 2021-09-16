using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Services
{
    public partial class SocketProxy
    {
        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public Task<MsResult<List<Entities.Version>>> GetVersions()
        {
            return HttpProxy.GetMessage<List<Entities.Version>>("api/version/get", null);
        }

        /// <summary>
        /// 添加版本
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Task<MsResult<int>> AddVersion(Entities.Version version)
        {
            return HttpProxy.PostMessage<int>("api/version/add", version);
        }

        /// <summary>
        /// 删除版本
        /// </summary>
        /// <param name="versionIDs"></param>
        /// <returns></returns>
        public Task<MsResult<bool>> RemoveVersions(List<string> versionIDs)
        {
            return HttpProxy.DeleteMessage<bool>("api/version/remove", new KeyValuePair<string, string>("versionIDs", string.Join(",", versionIDs)));
        }

        /// <summary>
        /// 修改版本
        /// </summary>
        /// <param name="versionIDs"></param>
        /// <returns></returns>
        public Task<MsResult<bool>> ModifyVersion(Entities.Version version)
        {
            return HttpProxy.PutMessage<bool>("api/version/modify", version);
        }

        /// <summary>
        /// 验证版本
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Task<MsResult<Entities.Version>> VerifyVersion(Entities.Version version)
        {
            return HttpProxy.PutMessage<Entities.Version>("api/version/verify", version);
        }
    }
}
