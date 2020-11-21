using System.Collections.Generic;
using System.Threading.Tasks;
using IRO.Mvc.Core.Dto;

namespace IRO.Mvc.Core.Services
{
    public class InMemoryRequestsLoggingService: IRequestsLoggingService
    {
        readonly List<HttpContextInfo> _records = new List<HttpContextInfo>();

        public async Task<int> SaveLogRecord(HttpContextInfo dto)
        {
            lock (_records)
            {
                dto.Id = _records.Count;
                _records.Add(dto);
                return dto.Id;
            }
        }

        public async Task<HttpContextInfo> GetRecord(int id)
        {
            return _records[id];
        }

        public async Task<IEnumerable<HttpContextInfo>> GetAllRecords()
        {
            return _records;
        }
    }
}