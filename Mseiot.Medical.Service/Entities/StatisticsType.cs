using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public enum StatisticsType
    {
        [Description("小时")]
        CurrentHour,
        [Description("本日")]
        CurrentDay,
        [Description("本周")]
        CurrenWeek,
        [Description("本月")]
        CurrentMonth,
        [Description("本月")]
        CurrentQuarter,
        [Description("本年")]
        CurrentYear,
        [Description("自定义")]
        Definition
    }
}
