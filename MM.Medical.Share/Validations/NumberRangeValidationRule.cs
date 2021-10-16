using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MM.Medical.Share.Validations
{
    public class NumberRangeValidationRule : ValidationRule
    {
        public string Title { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double dValue = 0;
            if (value != null && double.TryParse(value.ToString(), out dValue))
            {
                if (MaxValue >= dValue && MinValue <= dValue)
                    return new ValidationResult(true, null);
                return new ValidationResult(false, $"{ Title }取值范围为{ MinValue }-{ MaxValue }");
            }
            else return new ValidationResult(false, $"{ Title }数据格式异常");
        }
    }
}
