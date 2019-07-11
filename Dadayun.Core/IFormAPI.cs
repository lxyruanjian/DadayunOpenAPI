using Dadayun.Core.RequestDto;
using Dadayun.Core.ResponseDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    /// <summary>
    /// 单据API接口
    /// </summary>
    public interface IFormAPI
    {
        /// <summary>
        /// 获取单据实例列表。
        /// </summary>
        /// <typeparam name="T">集合项数据类型</typeparam>
        /// <param name="idOrName">单据模板 Id/名称/实体名称。</param>
        /// <param name="fields">返回字段；字段值与 keyOption有关。默认值为空，返回几个固定字段（Id,IsValid,IsBlock,DataEnable,Title,Creator,CreatorName,CreateTime,ModifyByName,ModifyTime）。</param>
        /// <param name="filter">筛选条件</param>
        /// <param name="start">分页开始索引。0-2147483647范围内某个整数值，默认值为0。</param>
        /// <param name="limit">每页数据。0-2147483647范围内某个整数值，默认值为20。</param>
        /// <param name="sort">排序字段(只支持单个字段，与 keyOption有关)，默认按创建时间CreateTime降序排序。</param>
        /// <param name="keyOption">提交的实例数据和返回的实例数据以什么为属性名（键名）。
        ///属性名：
        ///Entity 以实体属性名为属性名，默认；
        ///Caption 以[组名 -]控件名为属性名；
        ///Id 以字段的Id为属性名；
        ///FieldName 以字段的FieldName为属性名。</param>
        /// <returns>集合结果</returns>
        Task<SetResult<T>> GetFormInstancesAsync<T>(string idOrName, IEnumerable<string> fields = null, IEnumerable<QueryCondition> filter = null, int start = 0, int limit = 20, ISort sort = null, FormKeyOption keyOption = FormKeyOption.Entity, bool count = false);

        /// <summary>
        /// 获取指定单据实例数据
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="idOrName">单据模板 Id/名称/实体名称。</param>
        /// <param name="instanceId">单据实例Id</param>
        /// <param name="keyOption">提交的实例数据和返回的实例数据以什么为属性名（键名）。
        ///属性名：
        ///Entity 以实体属性名为属性名，默认；
        ///Caption 以[组名 -]控件名为属性名；
        ///Id 以字段的Id为属性名；
        ///FieldName 以字段的FieldName为属性名。</param>
        /// <param name="containsAuthority">是否包含权限</param>
        /// <returns></returns>
        Task<T> GetFormInstanceAsync<T>(string idOrName, Guid instanceId, FormKeyOption keyOption = FormKeyOption.Entity, bool containsAuthority = false);

        /// <summary>
        /// 新增单据实例数据
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="idOrName">单据模板 Id/名称/实体名称。</param>
        /// <param name="instanceId">单据实例Id</param>
        /// <param name="keyOption">提交的实例数据和返回的实例数据以什么为属性名（键名）。
        ///属性名：
        ///Entity 以实体属性名为属性名，默认；
        ///Caption 以[组名 -]控件名为属性名；
        ///Id 以字段的Id为属性名；
        ///FieldName 以字段的FieldName为属性名。</param>
        /// <param name="containsAuthority">是否包含权限</param>
        /// <returns></returns>
        Task<T> AddFormInstanceAsync<T>(string idOrName, object newInstance, FormKeyOption keyOption = FormKeyOption.Entity, bool containsAuthority = true);
    }
}
