using Ms.Libs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class MedicalTemplate : NotifyPropertyBase
    {
        private string modsee;
        private string moddia;
        private string name;
        private bool isSelected;
        private int medicalTemplateID;
        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public int MedicalTemplateID
        {
            get { return medicalTemplateID; }
            set
            {
                medicalTemplateID = value;
                RaisePropertyChanged("MedicalTemplateID");
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
        /// <summary>
        /// 镜下诊断
        /// </summary>
        public string Moddia
        {
            get { return moddia; }
            set
            {
                moddia = value;
                RaisePropertyChanged("Moddia");
            }
        }
        /// <summary>
        /// 内镜所见
        /// </summary>
        public string Modsee
        {
            get { return modsee; }
            set
            {
                modsee = value;
                RaisePropertyChanged("Modsee");
            }
        }
        [JsonIgnore]
        public ObservableCollection<MedicalTemplate> MedicalTemplates { get; set; } = new ObservableCollection<MedicalTemplate>();
    }
}
