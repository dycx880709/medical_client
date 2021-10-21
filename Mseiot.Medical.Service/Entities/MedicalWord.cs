﻿using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class MedicalWord : NotifyPropertyBase
    {
        private string name;
        public int MedicalWordID { get; set; }
        public int ParentID { get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        public ObservableCollection<MedicalWord> MedicalWords { get; set; }
    }
}
