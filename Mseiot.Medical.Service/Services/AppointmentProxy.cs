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

        public async Task<MsResult<List<Appointment>>> GetAppointments(DateTime startTime,DateTime endTime,string userInfo)
        {
            return await HttpProxy.GetMessage<List<Appointment>>("/api/Appointment/GetAppointments", new
            {
                StartTime=TimeHelper.ToUnixTime(startTime),
                EndTime= TimeHelper.ToUnixTime(endTime),
                UserInfo =userInfo
            });
        }

        public async Task<MsResult<int>> AddAppointment(Appointment appointment)
        {
            return await HttpProxy.PostMessage<int>("/api/Appointment/AddAppointment", appointment);
        }

        public async Task<MsResult<bool>> ModifyAppointment(Appointment appointment)
        {
            return await HttpProxy.PutMessage<bool>("/api/Appointment/ModifyAppointment", appointment);
        }

        public async Task<MsResult<bool>> RemoveAppointments(List<int> appointmentIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/Appointment/RemoveAppointments", new { ids = string.Join(",", appointmentIDs) });
        }

    }
}
