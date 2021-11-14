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
    public class Examination : NotifyPropertyBase
    {
        private string hiv;
        private string hcv;
        private string hBasg;
        private string anesthesia;
        private string collectCell;
        private string collectOrg;
        private string insertType;
        private string preoperative;
        private string result;
        private string bodyPose;
        private long examinationTime;
        private string bodyPart;
        private string clinicalDiagnosis;
        private string endoscopicFindings;
        private string microscopicDiagnosis;
        private string biopsySite;
        private string pathologicalDiagnosis;
        private string doctorAdvice;
        private long reportTime;
        private string doctorName;
        private int examinationID;
        private int endoscopeID;
        private Appointment appointment;
        private string patientTalk;
        private string examinationType;

        /// <summary>
        /// 检查ID
        /// </summary>
        public int ExaminationID
        {
            get { return examinationID; }
            set
            {
                examinationID = value;
                RaisePropertyChanged("ExaminationID");
            }
        }
        /// <summary>
        /// 关联预约ID
        /// </summary>
        public int AppointmentID { get; set; }
        /// <summary>
        /// 医生建议
        /// </summary>
        public string DoctorAdvice
        {
            get { return doctorAdvice; }
            set
            {
                doctorAdvice = value;
                RaisePropertyChanged("DoctorAdvice");
            }
        }
        /// <summary>
        /// 病理诊断
        /// </summary>
        public string PathologicalDiagnosis
        {
            get { return pathologicalDiagnosis; }
            set
            {
                pathologicalDiagnosis = value;
                RaisePropertyChanged("PathologicalDiagnosis");
            }
        }
        /// <summary>
        /// 活检部位
        /// </summary>
        public string BiopsySite
        {
            get { return biopsySite; }
            set
            {
                biopsySite = value;
                RaisePropertyChanged("BiopsySite");
            }
        }
        /// <summary>
        /// 镜下诊断
        /// </summary>
        public string MicroscopicDiagnosis
        {
            get { return microscopicDiagnosis; }
            set
            {
                microscopicDiagnosis = value;
                RaisePropertyChanged("MicroscopicDiagnosis");
            }
        }
        /// <summary>
        /// 内镜所见
        /// </summary>
        public string EndoscopicFindings
        {
            get { return endoscopicFindings; }
            set
            {
                endoscopicFindings = value;
                RaisePropertyChanged("EndoscopicFindings");
            }
        }
        /// <summary>
        /// 检查部位
        /// </summary>
        public string BodyPart
        {
            get { return bodyPart; }
            set
            {
                bodyPart = value;
                RaisePropertyChanged("BodyPart");
            }
        }
        /// <summary>
        /// 检查时间
        /// </summary>
        public long ExaminationTime
        {
            get { return examinationTime; }
            set
            {
                examinationTime = value;
                RaisePropertyChanged("ExaminationTime");
            }
        }
        /// <summary>
        /// 报告日期
        /// </summary>
        public long ReportTime
        {
            get { return reportTime; }
            set
            {
                reportTime = value;
                RaisePropertyChanged("ReportTime");
            }
        }
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string ClinicalDiagnosis
        {
            get { return clinicalDiagnosis; }
            set
            {
                clinicalDiagnosis = value;
                RaisePropertyChanged("ClinicalDiagnosis");
            }
        }
        /// <summary>
        /// 检查体位
        /// </summary>
        public string BodyPose
        {
            get { return bodyPose; }
            set
            {
                bodyPose = value;
                RaisePropertyChanged("BodyPose");
            }
        }
        /// <summary>
        /// 麻醉方法
        /// </summary>
        public string Anesthesia
        {
            get { return anesthesia; }
            set
            {
                anesthesia = value;
                RaisePropertyChanged("Anesthesia");
            }
        }
        /// <summary>
        /// 检查结果
        /// </summary>
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                RaisePropertyChanged("Result");
            }
        }
        /// <summary>
        /// 术前用药
        /// </summary>
        public string Preoperative
        {
            get { return preoperative; }
            set
            {
                preoperative = value;
                RaisePropertyChanged("Preoperative");
            }
        }
        /// <summary>
        /// 插入方式
        /// </summary>
        public string InsertType
        {
            get { return insertType; }
            set
            {
                insertType = value;
                RaisePropertyChanged("InsertType");
            }
        }
        /// <summary>
        /// 采集组织
        /// </summary>
        public string CollectOrg
        {
            get { return collectOrg; }
            set
            {
                collectOrg = value;
                RaisePropertyChanged("CollectOrg");
            }
        }
        /// <summary>
        /// 采集细胞
        /// </summary>
        public string CollectCell
        {
            get { return collectCell; }
            set
            {
                collectCell = value;
                RaisePropertyChanged("CollectCell");
            }
        }
        /// <summary>
        /// HIV
        /// </summary>
        public string HIV
        {
            get { return hiv; }
            set
            {
                hiv = value;
                RaisePropertyChanged("HIV");
            }
        }
        /// <summary>
        /// HBasg
        /// </summary>
        public string HBasg
        {
            get { return hBasg; }
            set
            {
                hBasg = value;
                RaisePropertyChanged("HBasg");
            }
        }
        /// <summary>
        /// HCV
        /// </summary>
        public string HCV
        {
            get { return hcv; }
            set
            {
                hcv = value;
                RaisePropertyChanged("HCV");
            }
        }
        /// <summary>
        /// 送检医生
        /// </summary>
        public string DoctorName
        {
            get { return doctorName; }
            set
            {
                doctorName = value;
                RaisePropertyChanged("DoctorName");
            }
        }
        /// <summary>
        /// 内镜ID
        /// </summary>
        public int EndoscopeID
        {
            get { return endoscopeID; }
            set
            {
                endoscopeID = value;
                RaisePropertyChanged("EndoscopeID");
            }
        }
        /// <summary>
        /// 主诉
        /// </summary>
        public string PatientTalk
        {
            get { return patientTalk; }
            set
            {
                patientTalk = value;
                RaisePropertyChanged("PatientTalk");
            }
        }
        /// <summary>
        /// 检查类型
        /// </summary>
        public string ExaminationType
        {
            get { return examinationType; }
            set
            {
                examinationType = value;
                RaisePropertyChanged("ExaminationType");
            }
        }
        /// <summary>
        /// 关联预约ID
        /// </summary>
        public Appointment Appointment
        {
            get { return appointment; }
            set 
            {
                appointment = value;
                RaisePropertyChanged("Appointment");
            }
        }
        public string DoctorID { get; set; }
        /// <summary>
        /// 检查诊室
        /// </summary>
        public string ConsultingName { get; set; }

        public ObservableCollection<ExaminationMedia> Images { get; set; }
        public ObservableCollection<ExaminationMedia> Videos { get; set; }
    }
}
