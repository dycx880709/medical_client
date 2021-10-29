using Ms.Libs.Models;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Services
{
    public partial class SocketProxy
    {
        public async Task<MsResult<ConsultingRoom>> LoginConsultingRoom(string consultingRoomName)
        {
            return await TcpProxy.SendMessage<ConsultingRoom>(Command.Module_System, Command.ConsultingRoom_Login, consultingRoomName);
        }
        public async Task<MsResult<int>> AddConsultingRoom(ConsultingRoom room)
        {
            return await HttpProxy.PostMessage<int>("/api/consulting/add", room);
        }

        public async Task<MsResult<bool>> RemoveConsultingRoom(int roomId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/consulting/remove", new KeyValuePair<string, string>("consultingroomid", roomId.ToString()));
        }

        public async Task<MsResult<List<ConsultingRoom>>> GetConsultingRooms()
        {
            return await HttpProxy.GetMessage<List<ConsultingRoom>>("/api/consulting/gets", null);
        }

        public async Task<MsResult<bool>> ModifyConsultingRoom(ConsultingRoom room)
        {
            return await HttpProxy.PutMessage<bool>("/api/consulting/modify", room);
        }

        public async Task<MsResult<bool>> AcceptConsultingRoom(int consultingRoomID, bool isUsed)
        {
            return await HttpProxy.PutMessage<bool>("/api/consulting/accept", new { ConsultingRoomID = consultingRoomID, IsUsed = isUsed });
        }

    }
}
