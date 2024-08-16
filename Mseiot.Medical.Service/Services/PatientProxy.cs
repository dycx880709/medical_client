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
        public async Task<MsResult<List<Patient>>> GetPatients(string patientNumber)
        {
            return await HttpProxy.GetMessage<List<Patient>>("/api/patient/gets", new { PatientNumber = patientNumber });
        }
    }
}
