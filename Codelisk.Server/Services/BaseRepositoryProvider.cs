using Codelisk.Server.Services.Interfaces;
using Prism.Magician;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Server.Services
{
    [RegisterSingleton(typeof(IBaseRepositoryProvider))]
    public class BaseRepositoryProvider : IBaseRepositoryProvider
    {
        public BaseRepositoryProvider()
        {
        }
        public string GetBaseUrl()
        {
            //format: https://google.com/
            string result = "";
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException();
            }
            return result;
        }
    }
}