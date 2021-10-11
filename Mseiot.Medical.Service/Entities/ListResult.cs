using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class ListResult<T> where T : class
    {
        /// <summary>
        /// 结果总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 结果集合
        /// </summary>
        public List<T> Results { get; set; }
    }
}
