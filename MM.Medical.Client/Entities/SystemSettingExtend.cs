using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{
    public class SystemSettingExtend : SystemSetting
    {
        private string cutshotSoundLocal;
        private string reportIconLocal;

        public string ReportIconLocal
        {
            get { return reportIconLocal; }
            set
            {
                reportIconLocal = value;
                RaisePropertyChanged("ReportIconLocal");
            }
        }

        public string CutshotSoundLocal
        {
            get { return cutshotSoundLocal; }
            set
            {
                cutshotSoundLocal = value;
                RaisePropertyChanged("CutshotSoundLocal");
            }
        }

    }
}
