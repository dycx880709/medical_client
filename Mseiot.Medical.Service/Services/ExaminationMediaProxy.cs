﻿using Ms.Libs.Models;
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
        public async Task<MsResult<bool>> ModifyExaminationMedia(ExaminationMedia media)
        {
            return await HttpProxy.PutMessage<bool>("/api/examination/modifymedia", media);
        }

        public async Task<MsResult<int>> AddExaminationMedia(ExaminationMedia media)
        {
            return await HttpProxy.PostMessage<int>("/api/examination/addmedia", media);
        }

        public async Task<MsResult<bool>> RemoveExaminationMedia(int examinationMediaId)
        {
            return await HttpProxy.DeleteMessage<bool>("/api/examination/removemedia", new { ExaminationMediaId = examinationMediaId });
        }
    }
}
