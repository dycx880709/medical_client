using Ms.Libs.Models;
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
        private int medicalWordID;
        private bool isSelected;

        public int MedicalWordID
        {
            get { return medicalWordID; }
            set
            {
                medicalWordID = value;
                RaisePropertyChanged("MedicalWordID");
            }
        }
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
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ObservableCollection<MedicalWord> MedicalWords { get; set; } = new ObservableCollection<MedicalWord>();
    }
}
