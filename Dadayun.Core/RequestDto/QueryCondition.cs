using System.Collections.Generic;

namespace Dadayun.Core.RequestDto
{
    public class QueryCondition
    {
        public QueryCondition(string id, QueryConditionOperator @operator, string value1 = null, string value2 = null)
        {
            Id = id;
            Operator = @operator;
            Value1 = value1;
            Value2 = value2;
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public QueryConditionOperator Operator { get; set; }

        /// <summary>
        /// 查询关键字，或范围查询开始区间
        /// </summary>
        public string Value1 { get; set; }

        /// <summary>
        /// 范围查询结束区间
        /// </summary>
        public string Value2 { get; set; }
    }

    public enum QueryConditionOperator
    {
        /// <summary>
        /// 包含（contains或like）
        /// </summary>
        cn,
        /// <summary>
        /// 等于（equal）
        /// </summary>
        eq,
        /// <summary>
        /// 不等于（not equal）
        /// </summary>
        ne,
        /// <summary>
        /// 大于（greater than）
        /// </summary>
        gt,
        /// <summary>
        /// 大于或等于（greater or equal）
        /// </summary>
        ge,
        /// <summary>
        /// 小于（less than）
        /// </summary>
        lt,
        /// <summary>
        /// 小于或等于（less or equal）
        /// </summary>
        le,

        /// <summary>
        /// 等于字符串空或null（is NullOrEmpty）
        /// </summary>
        nullOrEmpty,

        /// <summary>
        /// 大于或等于value1并且小于等于value2
        /// </summary>
        range
    }

    internal class QueryConditionRequest
    {
        public string Id { get; set; }

        public string Operator { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }
    }

    public static class QueryConditionExtensions
    {
        internal static IEnumerable<QueryConditionRequest> ToQueryConditionRequests(this IEnumerable<QueryCondition> cons)
        {
            var results = new List<QueryConditionRequest>();
            foreach (var item in cons)
            {
                results.Add(new QueryConditionRequest() { Id = item.Id, Operator = getRequestOperator(item.Operator), Value1 = item.Value1, Value2 = item.Value2 });
            }
            return results;
        }

        internal static string getRequestOperator(QueryConditionOperator @operator)
        {
            switch (@operator)
            {
                case QueryConditionOperator.cn:
                    return "like";
                case QueryConditionOperator.eq:
                    return "=";
                case QueryConditionOperator.ne:
                    return "<>";
                case QueryConditionOperator.gt:
                    return ">";
                case QueryConditionOperator.ge:
                    return ">=";
                case QueryConditionOperator.lt:
                    return "<";
                case QueryConditionOperator.le:
                    return "<=";
                case QueryConditionOperator.nullOrEmpty:
                    return "empty";
                case QueryConditionOperator.range:
                    return "range";
                default:
                    return "";
            }
        }
    }
}
