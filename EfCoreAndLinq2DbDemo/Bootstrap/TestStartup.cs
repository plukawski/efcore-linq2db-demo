using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindDataAccess;

namespace EfCoreAndLinq2DbDemo.Bootstrap
{
    public class TestStartup : Startup
    {
        private readonly IConfiguration configuration;
        public static SqliteConnection connection;

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
            connection = new SqliteConnection("Data Source=NorthwindInMemory;Mode=Memory;Cache=Shared");
            connection.Open();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            var testDbOptions = new DbContextOptionsBuilder<NorthwindContext>()
                .UseSqlite("Data Source=NorthwindInMemory;Mode=Memory;Cache=Shared")
                .Options;

            using (var context = new NorthwindContext(testDbOptions))
            {
                context.Database.EnsureCreated();
                AddTestData(context);
            }

            services.AddTransient<NorthwindContext>(ctx => new NorthwindContext(testDbOptions));
        }

        private void AddTestData(NorthwindContext context)
        {
            context.Categories.Add(new NorthwindDataAccess.Entities.Category() 
            {
                CategoryName = "Test category",
                Description = ""
            });

            context.Suppliers.Add(new NorthwindDataAccess.Entities.Supplier() 
            { 
                CompanyName = "Test company",
                ContactName = "Test contact",
            });

            var supplierHavingProducts = new NorthwindDataAccess.Entities.Supplier()
            {
                CompanyName = "New Orleans Cajun Delights",
                ContactName = "Test contact 2",
            };
            context.Suppliers.Add(supplierHavingProducts);

            for (int i = 15; i > 0; i--)
            {
                context.Products.Add(new NorthwindDataAccess.Entities.Product() 
                { 
                    Supplier = supplierHavingProducts,
                    ProductName = $"Test product {i}",
                    UnitPrice = i,
                    UnitsInStock = (short)i
                });
            }

            context.SaveChanges();
        }
    }
}
