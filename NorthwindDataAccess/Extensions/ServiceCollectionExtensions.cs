using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindDataAccess.Dao;

namespace NorthwindDataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNorthwindDemoDataAccess(this IServiceCollection services, string connectionString)
        {
            LinqToDBForEFTools.Initialize();

            services.AddTransient<NorthwindContext>((ctx) =>
            {
                var interceptor = ctx.GetService<NorthwindCommandInterceptor>();
                var options = new DbContextOptionsBuilder<NorthwindContext>()
                    .UseSqlServer(connectionString)
                    .AddInterceptors(interceptor)
                    .UseLinqToDb(options =>
                    {
                        options.AddInterceptor(interceptor);
                    })
                    .Options;
                return new NorthwindContext((DbContextOptions<NorthwindContext>)options);
            });
            services.AddSingleton<NorthwindCommandInterceptor>();

            services.AddTransient<IQueryDemoDataDao, QueryDemoDataDao>();
            services.AddTransient<ILinq2DbQueryDemoDataDao, Linq2DbQueryDemoDataDao>();
            services.AddTransient<IModifyDemoDataDao, ModifyDemoDataDao>();
        }
    }
}
