﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{

    public enum DecontaminateTaskStatus
    {
        [Description("等待")]
        Wait,
        [Description("完成")]
        Complete
    }


    public class DecontaminateTask:BaseModel
    {
        /// <summary>
        /// 消洗任务ID
        /// </summary>
        public int DecontaminateTaskID{ get; set; }

        /// <summary>
        /// 清洗员ID
        /// </summary>
        public string CleanUserID { get; set; }

        /// <summary>
        /// 清洗员
        /// </summary>
        public string CleanName { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorUserID { get; set; }

        /// <summary>
        /// 医生
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 患者身份证号
        /// </summary>
        public string PatientID { get; set; }

        /// <summary>
        /// 患者社保号
        /// </summary>
        public string PatientSI { get; set; }

        /// <summary>
        /// 内窥镜ID
        /// </summary>
        public int EndoscopeID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }


      


        /// <summary>
        /// 任务状态
        /// </summary>
        public DecontaminateTaskStatus DecontaminateTaskStatus { get; set; }

        /// <summary>
        /// 任务步骤
        /// </summary>
        public List<DecontaminateTaskStep> DecontaminateTaskSteps { get; set; } = new List<DecontaminateTaskStep>();
    }

    /// <summary>
    /// 清晰状态
    /// </summary>
    public enum DecontaminateStepStatus
    {
        [Description("等待")]
        Wait,
        [Description("清洗中")]
        Run,
        [Description("完成")]
        Complete
    }

    public class DecontaminateTaskStep:BaseModel
    {

        public int Index { get; set; }

        /// <summary>
        /// 步骤ID
        /// </summary>
        public int DecontaminateTaskStepID { get; set; }

        public int DecontaminateTaskID { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Name { get; set; }

        public int UseTime
        {
            get
            {
                return (int)(EndTime - StartTime);
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        private int residueTime = 0;
        /// <summary>
        /// 剩余时间
        /// </summary>
        public int ResidueTime 
        {
            get
            {
                return residueTime;
            }
            set
            {
                if (residueTime != value)
                {
                    residueTime = value;
                    NotifyPropertyChanged("ResidueTime");
                }
            }
        }


        /// <summary>
        /// RFID设备编号
        /// </summary>
        public int RFIDDeviceID { get; set; }


        /// <summary>
        /// RFID编号
        /// </summary>
        public int RFIDDeviceSN { get; set; }


        private DecontaminateStepStatus decontaminateStepStatus;
        /// <summary>
        /// 状态
        /// </summary>
        public DecontaminateStepStatus DecontaminateStepStatus
        {
            get
            {
                return decontaminateStepStatus;
            }
            set
            {
                if (decontaminateStepStatus != value)
                {
                    decontaminateStepStatus = value;
                    NotifyPropertyChanged("DecontaminateStepStatus");
                }
            }
        }
    }

    public class RFIDDevice: BaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int RFIDDeviceID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否在使用
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        /// 通信端口
        /// </summary>
        public string Com { get; set; }
    }

    public class Endoscope : BaseModel
    {

        private int endoscopeID;
        /// <summary>
        /// ID
        /// </summary>
        public int EndoscopeID 
        {
            get
            {
                return endoscopeID;
            }
            set
            {
                endoscopeID = value;
                NotifyPropertyChanged("EndoscopeID");
            }
        }

        /// <summary>
        /// 厂家
        /// </summary>
        public string Factory { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 采购时间
        /// </summary>
        public long Time { get; set; }
    }

    public class DecontaminateFlow : BaseModel
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public int DecontaminateFlowID { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        public string Factory { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
    }

    public class DecontaminateFlowStep : BaseModel
    {
        /// <summary>
        /// 步骤ID
        /// </summary>
        public int DecontaminateFlowStepID { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        public int DecontaminateFlowID { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public long Timeout { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int RFIDDeviceID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string RFIDDeviceName { get; set; }
    }
}
