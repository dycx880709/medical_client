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
            return await HttpProxy.PutMessage<int>("/api/booking/add", patient);
        }

        public async Task<MsResult<bool>> RemovePatient(int patientInfoId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/booking/remove", new KeyValuePair<string, string>("patientInfoId", patientInfoId.ToString()));
        }

        public async Task<MsResult<PatientInfo>> GePatientById(string patientInfoId)
        {
            return await HttpProxy.GetMessage<PatientInfo>("/api/booking/getpatientbyid", new KeyValuePair<string, string>("patientInfoId", patientInfoId.ToString()));
        }

        public async Task<MsResult<ListResult<PatientInfo>>> GePatients(int index, int count)
        {
            var dir = new Dictionary<string, string>();
            dir.Add("index", index.ToString());
            dir.Add("count", count.ToString());
            return await HttpProxy.GetMessage<ListResult<PatientInfo>>("/api/booking/gets", dir);
        }
    }
}
