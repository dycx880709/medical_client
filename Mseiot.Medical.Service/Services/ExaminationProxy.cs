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
        public async Task<MsResult<ListResult<Examination>>> GetExaminations(
            int index,
            int count,
            DateTime? startTime = null,
            DateTime? endTime = null,
            string userInfo = "",
            string doctorID = "",
            string examinationResult = "",
            string consultingName = "",
            string examinationType = "",
            string diagnoseInfo = ""
        )
        {
            return await HttpProxy.GetMessage<ListResult<Examination>>("/api/examination/getexaminations", new
            {
                Index = index.ToString(),
                Count = count.ToString(),
                StartTime = startTime != null ? TimeHelper.ToUnixTime(startTime.Value) : 0,
                EndTime = endTime != null ? TimeHelper.ToUnixTime(endTime.Value) : long.MaxValue,
                UserInfo = userInfo ?? "",
                DoctorID = doctorID,
                ExaminationResult = examinationResult,
                ConsultingName = consultingName,
                AppointmentType = examinationType,
                DiagnoseInfo = diagnoseInfo
            });
        }

        public async Task<MsResult<ListResult<Examination>>> GetBackRecords(
            int index,
            int count,
            DateTime? startTime,
            DateTime? endTime,
            string userInfo = "",
            string doctorID = "",
            string examinationType = ""
        )
        {
            return await HttpProxy.GetMessage<ListResult<Examination>>("/api/examination/getBackRecords", new
            {
                Index = index,
                Count = count,
                StartTime = TimeHelper.ToUnixTime(startTime.Value),
                EndTime = TimeHelper.ToUnixTime(endTime.Value),
                UserInfo = userInfo ?? "",
                DoctorID = doctorID,
                AppointmentType = examinationType,
            });
        }

        public async Task<MsResult<TimeResultCollection>> GetExaminationCountByTime(
            DateTime? startTime = null,
            DateTime? endTime = null,
            StatisticsType timeType = 0,
            string userInfo = "",
            string doctorID = "",
            string examinationResult = "",
            string consultingName = "",
            string examinationType = ""
        )
        {
            return await HttpProxy.GetMessage<TimeResultCollection>("/api/examination/getexaminationcountbytime", new
            {
                StartTime = startTime != null ? TimeHelper.ToUnixTime(startTime.Value) : 0,
                EndTime = endTime != null ? TimeHelper.ToUnixTime(endTime.Value) : long.MaxValue,
                TimeType = (int)timeType,
                UserInfo = userInfo ?? "",
                DoctorID = doctorID,
                ExaminationResult = examinationResult,
                ConsultingName = consultingName,
                AppointmentType = examinationType
        });
        }
            
        public async Task<MsResult<Examination>> GetExaminationsByAppointmentID(int appointmentID)
        {
            return await HttpProxy.GetMessage<Examination>("/api/examination/getexaminationbyappointmentid", new KeyValuePair<string, string>("appointmentID", appointmentID.ToString()));
        }

        public async Task<MsResult<bool>> ModifyExamination(Examination examination)
        {
            return await HttpProxy.PutMessage<bool>("/api/examination/modifyexamination", examination);
        }

        public async Task<MsResult<Examination>> StartExamination(int examinationID, string doctorID, int endoscopeID)
        {
            return await HttpProxy.GetMessage<Examination>("/api/examination/start", new 
            {
                ExaminationID = examinationID,
                DoctorID = doctorID,
                EndoscopeID = endoscopeID
            });
        }
        public async Task<MsResult<Examination>> ChangeEndoscope(int examinationID, int endoscopeID)
        {
            return await HttpProxy.GetMessage<Examination>("/api/examination/changeEndoscope", new
            {
                ExaminationID = examinationID,
                EndoscopeID = endoscopeID
            });
        }
        

        public async Task<MsResult<long>> StopExamination(int examinationID)
        {
            return await HttpProxy.GetMessage<long>("/api/examination/stop", new { ExaminationID = examinationID });
        }

        public async Task<MsResult<int>> AddExamination(Examination examination)
        {
            return await HttpProxy.PostMessage<int>("/api/examination/addexamination", examination);
        }
    }
}
