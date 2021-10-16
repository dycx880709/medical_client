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


    public class DecontaminateTask
    {
        /// <summary>
        /// 消洗任务ID
        /// </summary>
        public int DecontaminateTaskID{ get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

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
        public List<DecontaminateStep> DecontaminateSteps { get; set; } = new List<DecontaminateStep>();
    }

    /// <summary>
    /// 清晰状态
    /// </summary>
    public enum DecontaminateStepStatus
    {
        [Description("等待")]
        Wait,
        [Description("清洗中")]
        Normal,
        [Description("即将超时")]
        Warning,
        [Description("超时")]
        Timeout,
        [Description("完成")]
        Complete
    }

    public class DecontaminateStep
    {
        /// <summary>
        /// 步骤ID
        /// </summary>
        public int DecontaminateStepID { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Name { get; set; }

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
        public long Timeout { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public DecontaminateStepStatus DecontaminateStepStatus { get; set; }
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
