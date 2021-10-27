using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Core
{
    public static class Const
    {
        public static List<ValueLable> Sexs { get; set; } = new List<ValueLable>();

        static Const()
        {
            Sexs.Add(new ValueLable { Title = "男", Value = true });
            Sexs.Add(new ValueLable { Title = "女", Value = false });
        }
    }

    public class ValueLable
    {
        public string Title { get; set; }

        public object Value { get; set; }
    }
}
