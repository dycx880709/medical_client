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
        public async Task<MsResult<List<Appointment>>> GetTodayAppointmentRecords(string consultingRoomName)
        {
            return await HttpProxy.GetMessage<List<Appointment>>("/api/Appointment/GetTodayRecords", new { ConsultingRoomName = consultingRoomName });
        }

        public async Task<MsResult<Appointment>> GetAppointment(int appointmentID)
        {
            return await HttpProxy.GetMessage<Appointment>("/api/Appointment/GetAppointment", new { AppointmentID = appointmentID });
        }

        public async Task<MsResult<ListResult<Appointment>>> GetAppointments(
            int index, 
            int count, 
            DateTime? startTime = null, 
            DateTime? endTime = null, 
            string userInfo = "",
            string consultingRoomName = "",
            AppointmentStatus[] appointmentStatuses = null
        )
        {
            return await HttpProxy.GetMessage<ListResult<Appointment>>("/api/Appointment/GetAppointments", new
            {
                Index = index,
                Count = count,
                StartTime = startTime != null ? TimeHelper.ToUnixTime(startTime.Value) : 0,
                EndTime = endTime != null ? TimeHelper.ToUnixTime(endTime.Value) : long.MaxValue,
                UserInfo = userInfo ?? "",
                ConsultingRoomName = consultingRoomName,
                Statuses = appointmentStatuses != null ? string.Join(",", appointmentStatuses.Select(t => (int)t)) : null
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

        public async Task<MsResult<bool>> ModifyAppointmentStatus(Appointment appointment)
        {
            return await HttpProxy.PutMessage<bool>("/api/Appointment/ModifyAppointmentStatus", appointment);
        }

        public async Task<MsResult<bool>> RemoveAppointments(List<int> appointmentIDs)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/Appointment/RemoveAppointments", new { ids = string.Join(",", appointmentIDs) });
        }

    }
}
