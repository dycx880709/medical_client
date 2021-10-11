using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class EnumItems
    {
        public static List<string> MarryType { get; set; } = EnumHelper.GetDescriptions<MarryType>();
        public static List<string> Sex { get; set; } = EnumHelper.GetDescriptions<Sex>();
        public static List<string> AgeRange { get; set; } = EnumHelper.GetDescriptions<AgeRange>();
        public static List<string> DiagnoseType { get; set; } = EnumHelper.GetDescriptions<DiagnoseType>();
        public static List<string> ChargeType { get; set; } = EnumHelper.GetDescriptions<ChargeType>();
    }
}
