using Dadayun.Core;
using Dadayun.Core.Exceptions;
using System;
using System.Threading.Tasks;

namespace Dadayun.Sample
{
    public class BaseSample
    {
        private readonly IBaseAPI baseAPI;
        public BaseSample(IBaseAPI baseAPI)
        {
            this.baseAPI = baseAPI;
        }
        public async Task RunSample()
        {
            Console.WriteLine("获取服务器系统时间:");

            //获取服务器系统时间
            try
            {
                var currentDate = await baseAPI.GetDateAsync();

                Console.WriteLine(currentDate.ToLocalTime());
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
}
