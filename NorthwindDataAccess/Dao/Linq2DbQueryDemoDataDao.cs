using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using NorthwindDataAccess.Dto.QueryDataDemo;
using NorthwindDataAccess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindDataAccess.Dao
{
    public class Linq2DbQueryDemoDataDao : ILinq2DbQueryDemoDataDao
    {
        private readonly NorthwindContext context;

        public Linq2DbQueryDemoDataDao(NorthwindContext context)
        {
            this.context = context;
        }


        public async Task<List<PagingResults>> RowNumberDemoAsync()
        {
            var query1 = from p in context.Products
                         join s in context.Suppliers
                            on p.SupplierId equals s.SupplierId
                         where s.CompanyName == "New Orleans Cajun Delights"
                            || s.CompanyName == "Grandma Kelly's Homestead"
                         select new
                         {
                             //p.ProductId,
                             RowNumber = Sql.Ext.RowNumber()
                                .Over()
                                .PartitionBy(s.SupplierId)
                                .OrderBy(s.CompanyName)
                                .ToValue(),
                             UnitPrice = p.UnitPrice ?? 0,
                             p.ProductName
                         };

            return await query1
                .Where(x => x.RowNumber == 1)
                .Select(x => new PagingResults()
                {
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice
                })
                .ToLinqToDB()
                .ToListAsync();
        }

    }
}
