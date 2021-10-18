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
        public async Task<MsResult<int>> AddConsultingRoom(ConsultingRoom room)
        {
            return await HttpProxy.PostMessage<int>("/api/consulting/add", room);
        }

        public async Task<MsResult<bool>> RemoveConsultingRoom(int roomId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/consulting/remove", new KeyValuePair<string, string>("roomId", roomId.ToString()));
        }

        public async Task<MsResult<List<ConsultingRoom>>> GetConsultingRooms()
        {
            return await HttpProxy.GetMessage<List<ConsultingRoom>>("/api/consulting/gets", null);
        }

        public async Task<MsResult<bool>> ModifyConsultingRoom(ConsultingRoom room)
        {
            return await HttpProxy.PutMessage<bool>("/api/consulting/modify", room);
        }
    }
}
