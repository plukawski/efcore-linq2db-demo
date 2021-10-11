﻿using LinqToDB.EntityFrameworkCore;
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

            var options = new DbContextOptionsBuilder<NorthwindContext>()
                    .UseSqlServer(connectionString)
                    .Options;

            services.AddTransient<NorthwindContext>((ctx) => 
                new NorthwindContext(options, ctx.GetService<NorthwindCommandInterceptor>()));
            services.AddSingleton<NorthwindLinq2DbCommandProcessor>();
            services.AddSingleton<NorthwindCommandInterceptor>();

            services.AddTransient<IQueryDemoDataDao, QueryDemoDataDao>();
            services.AddTransient<ILinq2DbQueryDemoDataDao, Linq2DbQueryDemoDataDao>();
            services.AddTransient<IModifyDemoDataDao, ModifyDemoDataDao>();
        }
    }
}
