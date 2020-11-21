using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Mvc.Core.Dto;

namespace IRO.Mvc.Core.Services
{
    public interface IRequestsLoggingService
    {
        /// <summary>
        /// Return id.
        /// </summary>
        Task<int> SaveLogRecord(HttpContextInfo dto);

        Task<HttpContextInfo> GetRecord(int id);

        Task<IEnumerable<HttpContextInfo>> GetAllRecords();
    }
}
