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
        public async Task<MsResult<int>> AddBaseWord(BaseWord word)
        {
            return await HttpProxy.PutMessage<int>("/api/word/add", word);
        }

        public async Task<MsResult<bool>> RemoveBaseWord(int wordId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/word/remove", new KeyValuePair<string, string>("wordId", wordId.ToString()));
        }

        public async Task<MsResult<List<BaseWord>>> GeBaseWords(params string[] titles)
        {
            var condition = titles != null ? string.Join(",", titles) : null;
            return await HttpProxy.GetMessage<List<BaseWord>>("/api/word/getpatients", new KeyValuePair<string, string>("titles", condition));
        }

        public async Task<MsResult<bool>> ModifyBaseWord(BaseWord word)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/word/modify", word);
        }
    }
}
