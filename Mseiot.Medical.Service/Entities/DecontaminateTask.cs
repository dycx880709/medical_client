using Ms.Libs.SysLib;
using System;
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

    public class DecontaminateTaskExcel
    {
        /// <summary>
        /// 清洗员
        /// </summary>
        [ExcelHeader("清洗员")]
        public string CleanName { get; set; }
        /// <summary>
        /// 检查医生
        /// </summary>
        [ExcelHeader("检查医生")]
        public string DoctorName { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        [ExcelHeader("患者姓名")]
        public string PatientName { get; set; }
        /// <summary>
        /// 患者身份证号
        /// </summary>
        [ExcelHeader("身份证号")]
        public string PatientID { get; set; }
        /// <summary>
        /// 患者社保号
        /// </summary>
        [ExcelHeader("社保号码")]
        public string PatientSI { get; set; }
        /// <summary>
        /// 患者年龄
        /// </summary>
        [ExcelHeader("患者年龄")]
        public int PatientBirthday { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 任务步骤
        /// </summary>
        public List<DecontaminateTaskStep> DecontaminateTaskSteps { get; set; } = new List<DecontaminateTaskStep>();

        [ExcelHeader("开始时间")]
        public string StartTimeMsg{ get { return TimeHelper.FromUnixTime(this.StartTime).ToString("yyy/MM/dd HH:mm:ss"); }}
        [ExcelHeader("结束时间")]
        public string EndTimeMsg { get { return TimeHelper.FromUnixTime(this.EndTime).ToString("yyy/MM/dd HH:mm:ss"); } }
        [ExcelHeader("步骤一名称")]
        public string StepName1
        { 
            get 
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 0)
                    return this.DecontaminateTaskSteps[0].Name;
                return "";
            } 
        }
        [ExcelHeader("步骤一用时")]
        public int UseTime1
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 0)
                    return this.DecontaminateTaskSteps[0].UseTime;
                return 0;
            }
        }
        [ExcelHeader("步骤二名称")]
        public string StepName2
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 1)
                    return this.DecontaminateTaskSteps[1].Name;
                return "";
            }
        }
        [ExcelHeader("步骤二用时")]
        public int UseTime2
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 1)
                    return this.DecontaminateTaskSteps[1].UseTime;
                return 0;
            }
        }
        [ExcelHeader("步骤三名称")]
        public string StepName3
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 2)
                    return this.DecontaminateTaskSteps[2].Name;
                return "";
            }
        }
        [ExcelHeader("步骤三用时")]
        public int UseTime3
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 2)
                    return this.DecontaminateTaskSteps[2].UseTime;
                return 0;
            }
        }
        [ExcelHeader("步骤四名称")]
        public string StepName4
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 3)
                    return this.DecontaminateTaskSteps[3].Name;
                return "";
            }
        }
        [ExcelHeader("步骤四用时")]
        public int UseTime4
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 3)
                    return this.DecontaminateTaskSteps[3].UseTime;
                return 0;
            }
        }
        [ExcelHeader("步骤五名称")]
        public string StepName5
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 4)
                    return this.DecontaminateTaskSteps[4].Name;
                return "";
            }
        }
        [ExcelHeader("步骤五用时")]
        public int UseTime5
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 4)
                    return this.DecontaminateTaskSteps[4].UseTime;
                return 0;
            }
        }
        [ExcelHeader("步骤六名称")]
        public string StepName6
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 5)
                    return this.DecontaminateTaskSteps[5].Name;
                return "";
            }
        }
        [ExcelHeader("步骤六用时")]
        public int UseTime6
        {
            get
            {
                if (this.DecontaminateTaskSteps != null && this.DecontaminateTaskSteps.Count > 5)
                    return this.DecontaminateTaskSteps[5].UseTime;
                return 0;
            }
        }

        public DecontaminateTaskExcel(DecontaminateTask decontaminateTask)
        {
            decontaminateTask.CopyTo(this);
        }
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
        /// 检查医生
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
        /// 患者年龄
        /// </summary>
        public int PatientBirthday { get; set; }

        /// <summary>
        /// 检查ID
        /// </summary>
        public int ExaminationID { get; set; }

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
        /// 机身编号
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// 采购时间
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// 内窥镜状态
        /// </summary>
        public EndoscopeState State { get; set; }
    }

    public enum EndoscopeState
    { 
        [Description("待用")]
        Waiting,
        [Description("使用中")]
        Using,
        [Description("清洗中")]
        Decontaminating,
        [Description("停用")]
        Disabled
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
