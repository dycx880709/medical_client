using Ms.Libs.Models;
using Ms.Libs.SysLib;
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

        public async Task<MsResult<bool>> SignPatient(int patientInfoId)
        {
            return await HttpProxy.GetMessage<bool>("/api/booking/sign", new KeyValuePair<string, string>("patientInfoId", patientInfoId.ToString()));
        }

        public async Task<MsResult<bool>> UnSignPatient(int patientInfoId)
        {
            return await HttpProxy.GetMessage<bool>("/api/booking/unsign", new KeyValuePair<string, string>("patientInfoId", patientInfoId.ToString()));
        }

        public async Task<MsResult<PatientInfo>> GetPatientById(string patientInfoId)
        {
            return await HttpProxy.GetMessage<PatientInfo>("/api/booking/getpatientbyid", new KeyValuePair<string, string>("patientInfoId", patientInfoId.ToString()));
        }

        public async Task<MsResult<Patient>> GetPatientByCondition(string idCard = "", string socialId = "", string telphoneNumber = "")
        {
            if (string.IsNullOrEmpty(idCard) && string.IsNullOrEmpty(socialId) && string.IsNullOrEmpty(telphoneNumber))
                return new MsResult<Patient>();
            var dir = new Dictionary<string, string>();
            dir.Add("idCard", idCard);
            dir.Add("socialsecuritycode", socialId);
            dir.Add("telphoneNumber", telphoneNumber);
            return await HttpProxy.GetMessage<Patient>("/api/patient/getpatientbycondition", dir);
        }

        public async Task<MsResult<ListResult<PatientInfo>>> GetPatients(
            int index,
            int count,
            int createTime = 0,
            int checkTime = 0,
            string checkBody = "",
            string checkType = "",
            string className = "",
            string doctorName = "",
            string patientNumber = "",
            string patientName = "",
            string sex = "",
            string idCard = "",
            string telphoneNumber = "",
            string diagnoseType = "",
            string chargeType = "",
            PatientStatus[] patientStatuses = null)
        {
            var dir = new Dictionary<string, string>();
            dir.Add("index", index.ToString());
            dir.Add("count", count.ToString());
            dir.Add("createTime", createTime.ToString());
            dir.Add("checkTime", checkTime.ToString());
            dir.Add("checkBody", checkBody);
            dir.Add("checkType", checkType);
            dir.Add("className", className);
            dir.Add("doctorName", doctorName);
            dir.Add("patientNumber", patientNumber);
            dir.Add("patientName", patientName);
            dir.Add("sex", sex);
            dir.Add("idCard", idCard);
            dir.Add("telphoneNumber", telphoneNumber);
            dir.Add("diagnoseType", diagnoseType);
            dir.Add("chargeType", chargeType);
            if (patientStatuses != null)
                dir.Add("patientStatuses", string.Join(",", patientStatuses.Select(t => (int)t)));
            return await HttpProxy.GetMessage<ListResult<PatientInfo>>("/api/booking/gets", dir);
        }
    }
}
