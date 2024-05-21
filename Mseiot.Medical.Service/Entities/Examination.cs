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
        private string result;
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
        private string patientTalk;
        private string auditDoctor;
        private string nurse;
        private Appointment appointment;
        private ObservableCollection<ExaminationMedia> images;
        private ObservableCollection<ExaminationMedia> videos;

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
        /// 护士
        /// </summary>
        public string Nurse
        {
            get { return nurse; }
            set
            {
                nurse = value;
                RaisePropertyChanged("Nurse");
            }
        }
        /// <summary>
        /// 审核医生
        /// </summary>
        public string AuditDoctor
        {
            get { return auditDoctor; }
            set 
            {
                auditDoctor = value;
                RaisePropertyChanged("AuditDoctor");
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
        public Endoscope Endoscope { get; set; }

        public ObservableCollection<ExaminationMedia> Images
        {
            get { return images; }
            set 
            { 
                images = value;
                RaisePropertyChanged(nameof(Images));
            }
        }

        public ObservableCollection<ExaminationMedia> Videos
        {
            get { return videos; }
            set
            {
                videos = value;
                RaisePropertyChanged(nameof(Videos));
            }
        }
    }
}
