**DEMO application for using Linq2Db with EF Core**

This application demonstrates some most common scenarios when using a Linq2Db engine can greatly extend the abilities to projects based on Entity Framework Core.

Its main purpose is to show how easy it is to integrate Linq2Db to an existing, EF Core based, applications.

**Requirements**
1. Visual Studio 2019 or newer
1. SQL Server instance with Northwind database available under **`Northwind`** name and accessible using Windows Authentication of the current user (this application is using hardcoded `Server=localhost;Database=Northwind;Trusted_Connection=True` connection string to connect to SQL Server database). A script named `northwind.sql` included in this repository can be used to create mentioned database.
