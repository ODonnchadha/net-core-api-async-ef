## net-core-api-async-ef
- .NET Core 2.1 Web API using *best* async practices.
- AutoMapper extensions for ASP.NET Core are heavily embraced.
- See NOTES.MD for any related credit or blame.

## Run
- Simply compile and run. Entity Framework will seed your localdb.
- I use Microsoft SQL Server Management Studio with Server name: (localdb)\MSSQLLocalDB.

```csharp
services.AddDbContext<BooksContext>(c => c.UseSqlServer(Configuration["ConnectionStrings:LibraryConnectionString"]));

{
   "ConnectionStrings": {
      "LibraryConnectionString":  "Server=(localdb)\\mssqllocaldb;Database=Library;Trusted_Connection=True;"
   }
}
```