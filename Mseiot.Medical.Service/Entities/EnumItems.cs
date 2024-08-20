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
       public static List<string> DiagnoseType { get; set; } = EnumHelper.GetDescriptions<DiagnoseType>();
    }
}
