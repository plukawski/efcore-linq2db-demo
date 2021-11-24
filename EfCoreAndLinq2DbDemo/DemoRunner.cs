using EfCoreAndLinq2DbDemo.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NorthwindDataAccess.Dao;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfCoreAndLinq2DbDemo
{
    public class DemoRunner
    {
        private readonly IQueryDemoDataDao queryDataDao;
        private readonly IModifyDemoDataDao modifyDataDao;
        private readonly IServiceProvider container;

        public DemoRunner(
            IQueryDemoDataDao queryDao,
            IModifyDemoDataDao modifyDao,
            IServiceProvider container)
        {
            this.queryDataDao = queryDao;
            this.modifyDataDao = modifyDao;
            this.container = container;
            this.queryDataDao.WarmupOrms();
        }

        public async Task ProblematicQueryDemo()
        {
            var results = await queryDataDao.PagedResultsDemoAsync(0, 5);
            var results2 = await queryDataDao.PagedResultsDemoAsync(1, 5);
            foreach (var result in results)
            {
                Console.WriteLine($"{result.ProductName} - {result.UnitPrice}");
            }
            Console.WriteLine("--------");
            foreach (var result in results2)
            {
                Console.WriteLine($"{result.ProductName} - {result.UnitPrice}");
            }
        }

        public async Task UpdateLotOfRecordsDemo()
        {
            using (Stopper stopper = new Stopper("EF Core update"))
            {
                await modifyDataDao.UpdateEmployeesEfCoreAsync();
            }

            using (Stopper stopper = new Stopper("Linq2Db update"))
            {
                await modifyDataDao.UpdateEmployeesLinq2DbAsync();
            }
        }

        public async Task InsertLotOfRecordsDemo()
        {
            using (Stopper stopper = new Stopper("EF Core insertion"))
            {
                await modifyDataDao.InsertLotOfRecordsEfCoreAsync();
            }

            using (Stopper stopper = new Stopper("Linq2Db insertion"))
            {
                await modifyDataDao.InsertLotOfRecordsLinq2DbAsync();
            }
        }

        public async Task UpsertDemo()
        {
            IQueryDemoDataDao queryDataDao = container.GetService<IQueryDemoDataDao>();
            Random random = new Random();
            string productName = $"upsert_{random.Next()}";
            string wrongProductName = $"wrongupsert_{random.Next()}";

            var tasks = new List<Task>();
            using (Stopper stopper = new Stopper("Upsert"))
            {
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        IModifyDemoDataDao modifyDataDao = container.GetService<IModifyDemoDataDao>();
                        await modifyDataDao.UpsertProductDemoAsync(productName);
                    }));
                }
                await Task.WhenAll(tasks.ToArray());
            }
            
            var retryPolisy = Policy.Handle<Exception>().RetryAsync(3);
            tasks = new List<Task>();
            using (Stopper stopper = new Stopper("Wrong Upsert"))
            {
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        IModifyDemoDataDao modifyDataDao = container.GetService<IModifyDemoDataDao>();
                        await retryPolisy.ExecuteAsync(async () =>
                        {
                            await modifyDataDao.WrongUpsertProductDemoAsync(wrongProductName);
                        });
                    }));
                }

                try
                {
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wrong Upsert - Errors occured during execution:{ex.Message}");
                }
            }

            int productsCount = await queryDataDao.GetProductCountAsync(productName);
            int wrongProductsCount = await queryDataDao.GetProductCountAsync(wrongProductName);
            Console.WriteLine($"Upsert - number of products in db: {productsCount}");
            Console.WriteLine($"Wrong Upsert - number of products in db: {wrongProductsCount}");
        }

        public async Task Linq2DbVsSpDemo()
        {
            IQueryDemoDataDao queryDataDao = container.GetService<IQueryDemoDataDao>();
            Random random = new Random();
            string productName = $"upsert_{random.Next()}";
            string productSpName = $"upsert_sp_{random.Next()}";
            string warmupProductName = $"upsertwarmup_{random.Next()}";

            IModifyDemoDataDao warmupModifyDataDao = container.GetService<IModifyDemoDataDao>();
            await warmupModifyDataDao.UpsertProductDemoAsync(warmupProductName);
            await warmupModifyDataDao.UpsertProductDemoAsync(warmupProductName);
            await warmupModifyDataDao.UpsertProductSpDemoAsync(warmupProductName);


            var tasks = new List<Task>();
            using (Stopper stopper = new Stopper("Upsert SP"))
            {
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        IModifyDemoDataDao modifyDataDao = container.GetService<IModifyDemoDataDao>();
                        await modifyDataDao.UpsertProductSpDemoAsync(productSpName);
                    }));
                }
                try
                {
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Upsert SP - Errors occured during execution:{ex.Message}");
                }
            }
            
            tasks = new List<Task>();
            using (Stopper stopper = new Stopper("Upsert Linq2Db"))
            {
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        IModifyDemoDataDao modifyDataDao = container.GetService<IModifyDemoDataDao>();
                        await modifyDataDao.UpsertProductDemoLinq2DbOnlyAsync(productName);
                    }));
                }
                try
                {
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Upsert - Errors occured during execution:{ex.Message}");
                }
            }

            int productsCount = await queryDataDao.GetProductCountAsync(productName);
            int productsSpCount = await queryDataDao.GetProductCountAsync(productSpName);
            Console.WriteLine($"Upsert - number of products in db: {productsCount}");
            Console.WriteLine($"Upsert SP - number of products in db: {productsSpCount}");
        }

        public async Task OptionalParametersDemo()
        {
            await queryDataDao.FilterProductsAsync(null, "test");    //warmup
            await queryDataDao.FilterProductsSpAsync(null, "test");    //warmup

            using (Stopper stopper = new Stopper("Linq2Db filter products"))
            {
                await queryDataDao.FilterProductsAsync(null, "exotic");
            }
            
            using (Stopper stopper = new Stopper("SP filter products"))
            {
                await queryDataDao.FilterProductsSpAsync(null, "exotic");
            }
        }

        public async Task RowNumberDemo()
        {
            var results = await queryDataDao.RowNumberDemoAsync();
            foreach (var result in results)
            {
                Console.WriteLine($"{result.ProductName} - {result.UnitPrice}");
            }
        }

        public async Task PagedResultsWithTotalCountDemo()
        {
            var results = await queryDataDao.PagedResultsWithCountAllDemoAsync(0, 5);
            var results2 = await queryDataDao.PagedResultsWithCountAllDemoAsync(1, 5);
            foreach (var result in results)
            {
                Console.WriteLine($"{result.ProductName} - {result.UnitPrice} - {result.TotalCount}");
            }
            Console.WriteLine("--------");
            foreach (var result in results2)
            {
                Console.WriteLine($"{result.ProductName} - {result.UnitPrice} - {result.TotalCount}");
            }
        }
    }
}
