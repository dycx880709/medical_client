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
        public async Task<MsResult<int>> AddMedicalWord(MedicalWord medicalWord)
        {
            return await HttpProxy.PostMessage<int>("/api/medicalword/add", medicalWord);
        }

        public async Task<MsResult<bool>> RemoveMedicalWord(int medicalWordId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/medicalword/remove", new KeyValuePair<string, string>("medicalWordId", medicalWordId.ToString()));
        }

        public async Task<MsResult<List<MedicalWord>>> GetMedicalWords(params string[] names)
        {
            var condition = names != null ? string.Join(",", names) : null;
            return await HttpProxy.GetMessage<List<MedicalWord>>("/api/medicalword/gets", new KeyValuePair<string, string>("names", condition));
        }

        public async Task<MsResult<bool>> ModifyMedicalWord(MedicalWord medicalWord)
        {
            return await HttpProxy.PutMessage<bool>("/api/medicalword/modify", medicalWord);
        }
    }
}
