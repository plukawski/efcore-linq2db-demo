using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NorthwindDataAccess.Dto.QueryDataDemo;
using NorthwindDataAccess.Entities;
using NorthwindDataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindDataAccess.Dao
{
    public class QueryDemoDataDao : IQueryDemoDataDao
    {
        private readonly NorthwindContext context;

        public QueryDemoDataDao(NorthwindContext context)
        {
            this.context = context;
        }

        public async Task<List<PagingResults>> PagedResultsDemoAsync(int page, int pageSize)
        {
            var query1 = from p in context.Products
                         //from p in LinqToDB.LinqExtensions.With(LinqToDBForEFTools.ToLinqToDBTable(context.Products), "NOLOCK")
                         join s in context.Suppliers
                            on p.SupplierId equals s.SupplierId
                         where s.CompanyName == "New Orleans Cajun Delights" 
                            || s.CompanyName == "Grandma Kelly's Homestead"
                         select new
                         {
                             //p.ProductId,
                             UnitPrice = p.UnitPrice ?? 0,
                             p.ProductName
                         };

            return await query1
                .OrderBy(c => c.ProductName)
                .Distinct()
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(x => new PagingResults()
                {
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice
                })
                //.ToLinqToDB()
                .ToListAsync();
        }

        /*
         EF Core:
        exec sp_executesql N'SELECT [t].[ProductName], [t].[c] AS [UnitPrice]
        FROM (
            SELECT DISTINCT [p].[ProductID], COALESCE([p].[UnitPrice], 0.0) AS [c], [p].[ProductName]
            FROM [Products] AS [p]
            INNER JOIN [Suppliers] AS [s] ON [p].[SupplierID] = [s].[SupplierID]
            WHERE [s].[CompanyName] IN (N''New Orleans Cajun Delights'', N''Grandma Kelly''''s Homestead'')
        ) AS [t]
        ORDER BY (SELECT 1) --WTF is THAT?
        OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY',N'@__p_0 int,@__p_1 int',@__p_0=0,@__p_1=5
        */

        /*
         Linq2Db:
        exec sp_executesql N'SELECT
	        [t3].[ProductName],
	        [t3].[UnitPrice]
        FROM
	        (
		        SELECT
			        [t2].[UnitPrice],
			        [t2].[ProductName]
		        FROM
			        (
				        SELECT
					        [t1].[UnitPrice],
					        [t1].[ProductName],
					        ROW_NUMBER() OVER (ORDER BY [t1].[ProductName]) as [RN]
				        FROM
					        (
						        SELECT DISTINCT
							        Coalesce([x].[UnitPrice], 0) as [UnitPrice],
							        [x].[ProductName]
						        FROM
							        [Products] [x]
								        INNER JOIN [Suppliers] [s] ON [x].[SupplierID] = [s].[SupplierID]
						        WHERE
							        ([s].[CompanyName] = N''New Orleans Cajun Delights'' OR [s].[CompanyName] = N''Grandma Kelly''''s Homestead'')
					        ) [t1]
			        ) [t2]
		        WHERE
			        [t2].[RN] > @skip AND [t2].[RN] <= @take
	        ) [t3]
        ',N'@skip int,@take int',@skip=0,@take=5
         */

        public async Task<List<Product>> FilterProductsAsync(string productName, string supplierCompanyName)
        {
            var query = from p in context.Products
                            .ConditionalWhere(() => !string.IsNullOrWhiteSpace(productName), 
                                x => x.ProductName.StartsWith(productName))
                            .ConditionalWhere(() => !string.IsNullOrWhiteSpace(supplierCompanyName),
                                x => x.Supplier.CompanyName.StartsWith(supplierCompanyName))
                        select p;

            return await query
                .ToLinqToDB()
                .ToListAsync();
        }

        public async Task<List<Product>> FilterProductsSpAsync(string productName, string supplierCompanyName)
        {
            return await context.Products
                .FromSqlRaw("EXEC [dbo].[demo_FilterProducts] {0}, {1}", productName, supplierCompanyName)
                .ToListAsync();
        }
        /*
         CREATE PROCEDURE [dbo].[demo_FilterProducts]
	        -- Add the parameters for the stored procedure here
	        @productName NVARCHAR(40),
	        @supplierCompanyName NVARCHAR(40)
        AS
        BEGIN
	        -- SET NOCOUNT ON added to prevent extra result sets from
	        -- interfering with SELECT statements.
	        SET NOCOUNT ON;

            SELECT p.*
	        FROM Products p
	        LEFT JOIN Suppliers s on s.SupplierID = p.SupplierID
	        WHERE 1=1
	        AND (@productName IS NULL OR p.ProductName LIKE @productName+N'%')
	        AND (@supplierCompanyName IS NULL OR s.CompanyName LIKE @supplierCompanyName+N'%')
	
        END
         */

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
                             RowNumber = LinqToDB.AnalyticFunctions.RowNumber(LinqToDB.Sql.Ext)
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

        public async Task<List<PagingResultsWithTotalCount>> PagedResultsWithCountAllDemoAsync(int page, int pageSize)
        {
            var query1 = from p in context.Products
                         where p.ReorderLevel > 20
                         select new PagingResultsWithTotalCount
                         {
                             ProductId = p.ProductId,
                             TotalCount = LinqToDB.AnalyticFunctions.Count(LinqToDB.Sql.Ext)
                                .Over()
                                .ToValue(),
                             UnitPrice = p.UnitPrice ?? 0,
                             ProductName = p.ProductName
                         };

            return await query1
                .OrderBy(x => x.ProductId)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToLinqToDB()
                .ToListAsync();
        }
        /*
        exec sp_executesql N'SELECT
        	[p].[ProductID],
	        COUNT(*) OVER(),
	        IIF([p].[UnitPrice] IS NULL, 0, [p].[UnitPrice]),
	        [p].[ProductName]
        FROM
	        [Products] [p]
        WHERE
	        [p].[ReorderLevel] > 20
        ORDER BY
	        [p].[ProductID]
        OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY 
        ',N'@skip int,@take int',@skip=5,@take=5
         */

        public async Task<int> GetProductCountAsync(string productName)
        {
            return await context.Products.CountAsync(x => x.ProductName == productName);
        }

        public void WarmupOrms()
        {
            context.Products.Count();
            context.Products.ToLinqToDB().Count();
        }
    }
}
