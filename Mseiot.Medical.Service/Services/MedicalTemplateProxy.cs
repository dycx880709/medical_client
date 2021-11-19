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
        public async Task<MsResult<int>> AddMedicalTemplate(MedicalTemplate template)
        {
            return await HttpProxy.PostMessage<int>("/api/medicaltemplate/add", template);
        }

        public async Task<MsResult<bool>> RemoveMedicalTemplate(int medicalTemplateId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/medicaltemplate/remove", new KeyValuePair<string, string>("medicalTemplateId", medicalTemplateId.ToString()));
        }

        public async Task<MsResult<List<MedicalTemplate>>> GetMedicalTemplates(params string[] names)
        {
            var condition = names != null ? string.Join(",", names) : null;
            return await HttpProxy.GetMessage<List<MedicalTemplate>>("/api/medicaltemplate/gets", new KeyValuePair<string, string>("names", condition));
        }

        public async Task<MsResult<bool>> ModifyMedicalTemplate(MedicalTemplate template)
        {
            return await HttpProxy.PutMessage<bool>("/api/medicaltemplate/modify", template);
        }

        public async Task<MsResult<bool>> ImportMedicalTemplate(string jsonSource)
        { 
            return await HttpProxy.PostMessage<bool>("/api/medicaltemplate/import", jsonSource);
        }
    }
}
