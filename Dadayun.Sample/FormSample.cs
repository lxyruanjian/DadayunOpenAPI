using Dadayun.Core;
using Dadayun.Core.Exceptions;
using Dadayun.Core.RequestDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dadayun.Sample
{
    public class FormSample
    {
        private readonly IFormAPI formAPI;
        public FormSample(IFormAPI formAPI)
        {
            this.formAPI = formAPI;
        }

        public async Task RunSample()
        {

            try
            {
                Console.WriteLine("查询单据实例列表:");
                var cons = new List<QueryCondition>();
                //cons.Add(new QueryCondition("name", QueryConditionOperator.eq, "刘小勇"));
                var employeeSetResult = await formAPI.GetFormInstancesAsync<Dictionary<string,object>>("device_temp", filter: cons, keyOption: FormKeyOption.Entity, count: true);
            }
            catch (DDYClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (DDYServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //单据接口
            var employee = new Employee();
            employee.Title = "AlanTest_在职";
            employee.name = "AlanTest";
            employee.status = "在职";
            employee.email = "47003230@qq.com";

            //Guid newEmployeeId = Guid.Empty;
            //try
            //{
            //    Console.WriteLine("新建单据:");
            //    var newEmployee = await formAPI.AddFormInstanceAsync<Employee>("4BC85381-2209-4F36-A073-B1A7795BCD00", employee, FormKeyOption.Entity, false);
            //    newEmployeeId = newEmployee.Id;
            //}
            //catch (DDYClientException ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            //catch (DDYServerException ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            try
            {

                Console.WriteLine("获取单据实例数据:");
                var getEmployee = await formAPI.GetFormInstanceAsync<Employee>("device_temp", new Guid("68651903-6c4d-4ab9-982f-016e03f69c63"), FormKeyOption.Entity, false);
            }
            catch (DDYClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (DDYServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    /// <summary>
    /// 员工信息实体类
    /// </summary>
    public class Employee
    {
        public Guid Id { get; set; }

        public bool IsBlock { get; set; }

        public bool IsValid { get; set; }

        public bool DataEnable { get; set; }

        public string CreatorName { get; set; }

        public string ModifyByName { get; set; }

        public string CreatorPosition { get; set; }

        public string Title { get; set; }

        public string name { get; set; }

        public string status { get; set; }

        public string email { get; set; }
    }
}
