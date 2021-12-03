using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{

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
        /// 检查时间
        /// </summary>
        public long StartExamineTime { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public long EndExamineTime { get; set; }
        /// <summary>
        /// 任务步骤
        /// </summary>
        public List<DecontaminateTaskStep> DecontaminateTaskSteps { get; set; } = new List<DecontaminateTaskStep>();
        [ExcelHeader("清洗开始时间")]
        public string StartTimeMsg { get { return TimeHelper.FromUnixTime(this.StartTime).ToString("yyy/MM/dd HH:mm:ss"); } }
        [ExcelHeader("清洗结束时间")]
        public string EndTimeMsg { get { return TimeHelper.FromUnixTime(this.EndTime).ToString("yyy/MM/dd HH:mm:ss"); } }
        [ExcelHeader("检查开始时间")]
        public string StartExamineTimeMsg { get { return TimeHelper.FromUnixTime(this.StartExamineTime).ToString("yyy/MM/dd HH:mm:ss"); } }
        [ExcelHeader("检查结束时间")]
        public string EndExamineTimeMsg { get { return TimeHelper.FromUnixTime(this.EndExamineTime).ToString("yyy/MM/dd HH:mm:ss"); } }
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

}
