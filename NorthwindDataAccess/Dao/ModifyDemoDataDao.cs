﻿using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NorthwindDataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindDataAccess.Dao
{
    public class ModifyDemoDataDao : IModifyDemoDataDao
    {
        private readonly NorthwindContext context;

        public ModifyDemoDataDao(NorthwindContext context)
        {
            this.context = context;
        }

        public async Task UpdateEmployeesEfCoreAsync()
        {
            var updateQuery = from e in context.Employees
                              where e.HireDate > new DateTime(2021, 08, 01)
                              select e;

            var employeesToUpdate = await updateQuery.ToListAsync();
            foreach (var employee in employeesToUpdate)
            {
                employee.HireDate = employee.HireDate?.AddSeconds(1);
            }
            await context.SaveChangesAsync();
        }
        /*
         EF core generates:
        SELECT [e].[EmployeeID], [e].[Address], [e].[BirthDate], [e].[City], [e].[Country], [e].[Extension], [e].[FirstName], [e].[HireDate], [e].[HomePhone], [e].[LastName], [e].[Notes], [e].[Photo], [e].[PhotoPath], [e].[PostalCode], [e].[Region], [e].[ReportsTo], [e].[Title], [e].[TitleOfCourtesy]
        FROM [Employees] AS [e]
        WHERE [e].[HireDate] > '2021-08-01T00:00:00.000'

        And over 6 thousands:
        UPDATE [Employees] SET [HireDate] = @p82
        WHERE [EmployeeID] = @p83;
        SELECT @@ROWCOUNT;
         */

        public async Task UpdateEmployeesEfCore7Async()
        {
            var updateQuery = from e in context.Employees
                              where e.HireDate > new DateTime(2021, 08, 01)
                              select e;

            var employeesToUpdate = await updateQuery.ExecuteUpdateAsync(x =>
                x.SetProperty(
                    b => b.HireDate,
                    b => b.HireDate.HasValue ? b.HireDate.Value.AddSeconds(1) : null
                    ));
        }
        /*
        EF core 7 generates:
        UPDATE [e]
        SET [e].[HireDate] = CASE
            WHEN [e].[HireDate] IS NOT NULL THEN DATEADD(second, CAST(1.0E0 AS int), [e].[HireDate])
            ELSE NULL
        END
        FROM [Employees] AS [e]
        WHERE [e].[HireDate] > '2021-08-01T00:00:00.000'
         */

        public async Task UpdateEmployeesLinq2DbAsync()
        {
            var updateQuery = from e in context.Employees
                              where e.HireDate > new DateTime(2021, 08, 01)
                              select e;

            await LinqToDB.LinqExtensions.UpdateAsync(updateQuery.ToLinqToDB(), x => new Entities.Employee()
            {
                HireDate = x.HireDate.HasValue ? x.HireDate.Value.AddSeconds(1) : null
            });
        }
        /*
        Linq2Db Generates
         exec sp_executesql N'UPDATE
	        [e]
        SET
	        [e].[HireDate] = CASE
		        WHEN [e].[HireDate] IS NOT NULL
			        THEN DateAdd(second, 1, [e].[HireDate])
		        ELSE NULL
	        END
        FROM
	        [Employees] [e]
        WHERE
	        [e].[HireDate] > @HireDate
        ',N'@HireDate datetime',@HireDate='2021-08-01 00:00:00'
         */

        public async Task InsertLotOfRecordsEfCoreAsync()
        {
            List<Employee> newEmployees = new List<Employee>();
            for (int i=1; i < 1000; i++)
            {
                newEmployees.Add(new Employee() 
                { 
                    FirstName = $"efcore",
                    LastName = $"Last name {i}",
                    HireDate = (new DateTime(2016, 1, 1)).AddSeconds(i)
                });
            }

            context.Employees.AddRange(newEmployees);
            await context.SaveChangesAsync();
        }
        /*
         EF core generates several MERGE operations
        exec sp_executesql N'SET NOCOUNT ON;
DECLARE @inserted0 TABLE ([EmployeeID] int, [_Position] [int]);
MERGE [Employees] USING (
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, 0),
(@p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25, @p26, @p27, @p28, @p29, @p30, @p31, @p32, @p33, 1),
(@p34, @p35, @p36, @p37, @p38, @p39, @p40, @p41, @p42, @p43, @p44, @p45, @p46, @p47, @p48, @p49, @p50, 2),
(@p51, @p52, @p53, @p54, @p55, @p56, @p57, @p58, @p59, @p60, @p61, @p62, @p63, @p64, @p65, @p66, @p67, 3),
(@p68, @p69, @p70, @p71, @p72, @p73, @p74, @p75, @p76, @p77, @p78, @p79, @p80, @p81, @p82, @p83, @p84, 4),
(@p85, @p86, @p87, @p88, @p89, @p90, @p91, @p92, @p93, @p94, @p95, @p96, @p97, @p98, @p99, @p100, @p101, 5),
(@p102, @p103, @p104, @p105, @p106, @p107, @p108, @p109, @p110, @p111, @p112, @p113, @p114, @p115, @p116, @p117, @p118, 6),
(@p119, @p120, @p121, @p122, @p123, @p124, @p125, @p126, @p127, @p128, @p129, @p130, @p131, @p132, @p133, @p134, @p135, 7),
(@p136, @p137, @p138, @p139, @p140, @p141, @p142, @p143, @p144, @p145, @p146, @p147, @p148, @p149, @p150, @p151, @p152, 8),
(@p153, @p154, @p155, @p156, @p157, @p158, @p159, @p160, @p161, @p162, @p163, @p164, @p165, @p166, @p167, @p168, @p169, 9),
(@p170, @p171, @p172, @p173, @p174, @p175, @p176, @p177, @p178, @p179, @p180, @p181, @p182, @p183, @p184, @p185, @p186, 10),
(@p187, @p188, @p189, @p190, @p191, @p192, @p193, @p194, @p195, @p196, @p197, @p198, @p199, @p200, @p201, @p202, @p203, 11),
(@p204, @p205, @p206, @p207, @p208, @p209, @p210, @p211, @p212, @p213, @p214, @p215, @p216, @p217, @p218, @p219, @p220, 12),
(@p221, @p222, @p223, @p224, @p225, @p226, @p227, @p228, @p229, @p230, @p231, @p232, @p233, @p234, @p235, @p236, @p237, 13),
(@p238, @p239, @p240, @p241, @p242, @p243, @p244, @p245, @p246, @p247, @p248, @p249, @p250, @p251, @p252, @p253, @p254, 14),
(@p255, @p256, @p257, @p258, @p259, @p260, @p261, @p262, @p263, @p264, @p265, @p266, @p267, @p268, @p269, @p270, @p271, 15),
(@p272, @p273, @p274, @p275, @p276, @p277, @p278, @p279, @p280, @p281, @p282, @p283, @p284, @p285, @p286, @p287, @p288, 16),
(@p289, @p290, @p291, @p292, @p293, @p294, @p295, @p296, @p297, @p298, @p299, @p300, @p301, @p302, @p303, @p304, @p305, 17),
(@p306, @p307, @p308, @p309, @p310, @p311, @p312, @p313, @p314, @p315, @p316, @p317, @p318, @p319, @p320, @p321, @p322, 18),
(@p323, @p324, @p325, @p326, @p327, @p328, @p329, @p330, @p331, @p332, @p333, @p334, @p335, @p336, @p337, @p338, @p339, 19),
(@p340, @p341, @p342, @p343, @p344, @p345, @p346, @p347, @p348, @p349, @p350, @p351, @p352, @p353, @p354, @p355, @p356, 20),
(@p357, @p358, @p359, @p360, @p361, @p362, @p363, @p364, @p365, @p366, @p367, @p368, @p369, @p370, @p371, @p372, @p373, 21),
(@p374, @p375, @p376, @p377, @p378, @p379, @p380, @p381, @p382, @p383, @p384, @p385, @p386, @p387, @p388, @p389, @p390, 22),
(@p391, @p392, @p393, @p394, @p395, @p396, @p397, @p398, @p399, @p400, @p401, @p402, @p403, @p404, @p405, @p406, @p407, 23),
(@p408, @p409, @p410, @p411, @p412, @p413, @p414, @p415, @p416, @p417, @p418, @p419, @p420, @p421, @p422, @p423, @p424, 24),
(@p425, @p426, @p427, @p428, @p429, @p430, @p431, @p432, @p433, @p434, @p435, @p436, @p437, @p438, @p439, @p440, @p441, 25),
(@p442, @p443, @p444, @p445, @p446, @p447, @p448, @p449, @p450, @p451, @p452, @p453, @p454, @p455, @p456, @p457, @p458, 26),
(@p459, @p460, @p461, @p462, @p463, @p464, @p465, @p466, @p467, @p468, @p469, @p470, @p471, @p472, @p473, @p474, @p475, 27),
(@p476, @p477, @p478, @p479, @p480, @p481, @p482, @p483, @p484, @p485, @p486, @p487, @p488, @p489, @p490, @p491, @p492, 28),
(@p493, @p494, @p495, @p496, @p497, @p498, @p499, @p500, @p501, @p502, @p503, @p504, @p505, @p506, @p507, @p508, @p509, 29),
(@p510, @p511, @p512, @p513, @p514, @p515, @p516, @p517, @p518, @p519, @p520, @p521, @p522, @p523, @p524, @p525, @p526, 30),
(@p527, @p528, @p529, @p530, @p531, @p532, @p533, @p534, @p535, @p536, @p537, @p538, @p539, @p540, @p541, @p542, @p543, 31),
(@p544, @p545, @p546, @p547, @p548, @p549, @p550, @p551, @p552, @p553, @p554, @p555, @p556, @p557, @p558, @p559, @p560, 32),
(@p561, @p562, @p563, @p564, @p565, @p566, @p567, @p568, @p569, @p570, @p571, @p572, @p573, @p574, @p575, @p576, @p577, 33),
(@p578, @p579, @p580, @p581, @p582, @p583, @p584, @p585, @p586, @p587, @p588, @p589, @p590, @p591, @p592, @p593, @p594, 34),
(@p595, @p596, @p597, @p598, @p599, @p600, @p601, @p602, @p603, @p604, @p605, @p606, @p607, @p608, @p609, @p610, @p611, 35),
(@p612, @p613, @p614, @p615, @p616, @p617, @p618, @p619, @p620, @p621, @p622, @p623, @p624, @p625, @p626, @p627, @p628, 36),
(@p629, @p630, @p631, @p632, @p633, @p634, @p635, @p636, @p637, @p638, @p639, @p640, @p641, @p642, @p643, @p644, @p645, 37),
(@p646, @p647, @p648, @p649, @p650, @p651, @p652, @p653, @p654, @p655, @p656, @p657, @p658, @p659, @p660, @p661, @p662, 38),
(@p663, @p664, @p665, @p666, @p667, @p668, @p669, @p670, @p671, @p672, @p673, @p674, @p675, @p676, @p677, @p678, @p679, 39),
(@p680, @p681, @p682, @p683, @p684, @p685, @p686, @p687, @p688, @p689, @p690, @p691, @p692, @p693, @p694, @p695, @p696, 40),
(@p697, @p698, @p699, @p700, @p701, @p702, @p703, @p704, @p705, @p706, @p707, @p708, @p709, @p710, @p711, @p712, @p713, 41)) AS i ([Address], [BirthDate], [City], [Country], [Extension], [FirstName], [HireDate], [HomePhone], [LastName], [Notes], [Photo], [PhotoPath], [PostalCode], [Region], [ReportsTo], [Title], [TitleOfCourtesy], _Position) ON 1=0
WHEN NOT MATCHED THEN
INSERT ([Address], [BirthDate], [City], [Country], [Extension], [FirstName], [HireDate], [HomePhone], [LastName], [Notes], [Photo], [PhotoPath], [PostalCode], [Region], [ReportsTo], [Title], [TitleOfCourtesy])
VALUES (i.[Address], i.[BirthDate], i.[City], i.[Country], i.[Extension], i.[FirstName], i.[HireDate], i.[HomePhone], i.[LastName], i.[Notes], i.[Photo], i.[PhotoPath], i.[PostalCode], i.[Region], i.[ReportsTo], i.[Title], i.[TitleOfCourtesy])
OUTPUT INSERTED.[EmployeeID], i._Position
INTO @inserted0;

SELECT [t].[EmployeeID] FROM [Employees] t
INNER JOIN @inserted0 i ON ([t].[EmployeeID] = [i].[EmployeeID])
ORDER BY [i].[_Position];

'
         */

        public async Task InsertLotOfRecordsLinq2DbAsync()
        {
            List<Employee> newEmployees = new List<Employee>();
            for (int i = 1; i < 1000; i++)
            {
                newEmployees.Add(new Employee()
                {
                    FirstName = $"linq2db",
                    LastName = $"Last name {i}",
                    HireDate = (new DateTime(2016, 1, 1)).AddSeconds(i)
                });
            }
            await context.BulkCopyAsync(new LinqToDB.Data.BulkCopyOptions(), newEmployees);
        }
        /*
         Linq2Db generates single INSERT BULK operation:
        insert bulk [Employees] ([LastName] NVarChar(20) COLLATE Latin1_General_CI_AI, [FirstName] NVarChar(10) COLLATE Latin1_General_CI_AI, [Title] NVarChar(30) COLLATE Latin1_General_CI_AI, [TitleOfCourtesy] NVarChar(25) COLLATE Latin1_General_CI_AI, [BirthDate] DateTime, [HireDate] DateTime, [Address] NVarChar(60) COLLATE Latin1_General_CI_AI, [City] NVarChar(15) COLLATE Latin1_General_CI_AI, [Region] NVarChar(15) COLLATE Latin1_General_CI_AI, [PostalCode] NVarChar(10) COLLATE Latin1_General_CI_AI, [Country] NVarChar(15) COLLATE Latin1_General_CI_AI, [HomePhone] NVarChar(24) COLLATE Latin1_General_CI_AI, [Extension] NVarChar(4) COLLATE Latin1_General_CI_AI, [Photo] Image, [Notes] NText COLLATE Latin1_General_CI_AI, [ReportsTo] Int, [PhotoPath] NVarChar(255) COLLATE Latin1_General_CI_AI)
         */

        public async Task UpsertProductDemoAsync(string productName)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var updateQuery = context.Products.Where(x => x.ProductName == productName);
            int rowsAffected = await LinqToDB.LinqExtensions
                .UpdateAsync(updateQuery.ToLinqToDB(), x => new Product() 
            {
                UnitsInStock = x.UnitsInStock.HasValue ? (short)(x.UnitsInStock.Value + 1) : (short)0
            });

            if (rowsAffected == 0)
            {
                var product = new Product()
                {
                    ProductName = productName,
                    UnitsInStock = 0,
                    CategoryId = 1,
                    SupplierId = 1,
                    UnitPrice = 1,
                    UnitsOnOrder = 0,
                    QuantityPerUnit = "1",
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();
            }
            
            await transaction.CommitAsync();
        }

        public async Task UpsertProductDemoLinq2DbOnlyAsync(string productName)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var updateQuery = context.Products.Where(x => x.ProductName == productName);
            int rowsAffected = await LinqToDB.LinqExtensions
                .UpdateAsync(updateQuery.ToLinqToDB(), x => new Product()
                {
                    UnitsInStock = x.UnitsInStock.HasValue ? (short)(x.UnitsInStock.Value + 1) : (short)0
                });

            if (rowsAffected == 0)
            {
                await LinqToDB.LinqExtensions
                    .InsertAsync(context.Products.ToLinqToDBTable(), () => new Product()
                    {
                        ProductName = productName,
                        UnitsInStock = 0,
                        CategoryId = 1,
                        SupplierId = 1,
                        UnitPrice = 1,
                        UnitsOnOrder = 0,
                        QuantityPerUnit = "1",
                    });
            }

            await transaction.CommitAsync();
        }

        public async Task UpsertProductSpDemoAsync(string productName)
        {
            await context.Database.ExecuteSqlRawAsync("EXEC demo_UpsertProduct {0}", productName);
            /*
             ALTER PROCEDURE demo_UpsertProduct
	            @productName NVARCHAR(40)
            AS
            BEGIN
	            -- SET NOCOUNT ON added to prevent extra result sets from
	            -- interfering with SELECT statements.
	            SET NOCOUNT ON;

	            BEGIN TRANSACTION
	            SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                UPDATE p SET p.UnitsInStock = IIF(p.UnitsInStock IS NOT NULL, Convert(SmallInt,p.UnitsInStock + 1), 0)
	            FROM Products p
	            WHERE ProductName = @productName

	            IF (@@ROWCOUNT = 0)
	            BEGIN
		            INSERT INTO Products (ProductName, UnitsInStock, CategoryID, SupplierID, UnitPrice, UnitsOnOrder, QuantityPerUnit)
		            VALUES (@productName, 0, 1, 1, 1, 0, '1')
	            END
	
	            COMMIT
            END
             */
        }

        public async Task WrongUpsertProductDemoAsync(string productName)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var existingProduct = await context.Products.Where(x => x.ProductName == productName).FirstOrDefaultAsync();

            if (existingProduct == null)
            {
                var product = new Product()
                {
                    ProductName = productName,
                    UnitsInStock = 0,
                    CategoryId = 1,
                    SupplierId = 1,
                    UnitPrice = 1,
                    UnitsOnOrder = 0,
                    QuantityPerUnit = "1",
                };

                context.Products.Add(product);
            }
            else
            {
                existingProduct.UnitsInStock = existingProduct.UnitsInStock.HasValue
                    ? (short)(existingProduct.UnitsInStock.Value + 1)
                    : (short)0;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
