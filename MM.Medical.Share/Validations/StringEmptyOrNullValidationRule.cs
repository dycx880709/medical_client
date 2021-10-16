using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MM.Medical.Share.Validations
{
    public class StringLengthValidationRule : ValidationRule
    {
        public string Title { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, $"{Title}不能为空");
            else
            {
                var str = value.ToString();
                if (MinValue.HasValue && MaxValue.HasValue)
                {
                    if (MinValue.Value > str.Length || MaxValue.Value < str.Length)
                        return new ValidationResult(false, $"{Title}取值范围为{ MinValue }-{ MaxValue }");
                }
                else if (!MinValue.HasValue && MaxValue.HasValue)
                {
                    if (MaxValue.Value < str.Length)
                        return new ValidationResult(false, $"{Title}取值不能大于{ MaxValue }");
                }
                else if (MinValue.HasValue && !MaxValue.HasValue)
                {
                    if (MinValue.Value > str.Length)
                        return new ValidationResult(false, $"{Title}取值不能小于{ MinValue }");
                }
            }
            return new ValidationResult(true, null);
        }
    }
}
