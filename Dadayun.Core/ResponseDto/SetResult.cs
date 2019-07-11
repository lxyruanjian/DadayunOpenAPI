using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dadayun.Core.ResponseDto
{
    /// <summary>
    /// 集合结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetResult<T>
    {
        /// <summary>
        /// 分页开始索引。
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 每页数据。
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 集合数据
        /// </summary>
        public IEnumerable<T> Datas { get; set; }
        /// <summary>
        /// 集合大小
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int? TotalCount { get; set; }
    }
}
