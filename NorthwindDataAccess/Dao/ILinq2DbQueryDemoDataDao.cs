using NorthwindDataAccess.Dto.QueryDataDemo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthwindDataAccess.Dao
{
    public interface ILinq2DbQueryDemoDataDao
    {
        Task<List<PagingResults>> RowNumberDemoAsync();
    }
}