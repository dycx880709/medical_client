using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class AppointmentRecord
    {
        public Appointment CurAppointment { get; set; }
        public List<Examination> Examinations { get; set; }
    }
}
