﻿using Ms.Libs.Models;
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
            string userInfo = ""
        )
        {
            return await HttpProxy.GetMessage<ListResult<Examination>>("/api/Examination/GetExaminations", new
            {
                Index = index.ToString(),
                Count = count.ToString(),
                StartTime = startTime != null ? TimeHelper.ToUnixTime(startTime.Value) : 0,
                EndTime = endTime != null ? TimeHelper.ToUnixTime(endTime.Value) : long.MaxValue,
                UserInfo = userInfo ?? "",
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
    }
}
