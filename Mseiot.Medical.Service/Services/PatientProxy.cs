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
        public async Task<MsResult<int>> AddPatient(PatientInfo patient)
        {
            return await HttpProxy.PutMessage<int>("/api/patient/add", patient);
        }

        public async Task<MsResult<bool>> RemovePatient(int patientId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/patient/remove", new KeyValuePair<string, string>("patientId", patientId.ToString()));
        }

        public async Task<MsResult<PatientInfo>> GePatient(string id)
        {
            return await HttpProxy.GetMessage<PatientInfo>("/api/patient/getpatientbyid", id);
        }

        public async Task<MsResult<ListResult<PatientInfo>>> GePatients(int index, int count)
        {
            var dir = new Dictionary<string, string>();
            dir.Add("index", index.ToString());
            dir.Add("count", count.ToString());
            return await HttpProxy.GetMessage<ListResult<PatientInfo>>("/api/patient/gets", dir);
        }
    }
}
