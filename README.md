# ODataHelper.AspNetCore.EntityFrameworkCore
ODataHelper simplifies adding OData functionality to your AspNetCore application by providing Controllers and Repository Adapters.

## Install

### Package Manager Console

```
PM> Install-Package ODataHelper.AspNetCore.EntityFrameworkCore
```

### .NET CLI Console

```
> dotnet add package ODataHelper.AspNetCore.EntityFrameworkCore
```

## Quick Usage Guide with AspNetCore and EFCore

### Define Entity Model

```
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

### Define DbContext

```
public class MyDbContext : DbContext
{
    DbSet<User> Users;
}
```

### Inherit EfCoreODataRepositoryAdapter

```
public class MyODataRepositoryAdapter<TEntity> : EfCoreODataRepositoryAdapter<MyDbContext,TEntity>
{
    // No other code needed for out-of-box support
}
```

### Inherit ODataHelperController

```
public class UsersController : ODataHelperController<User>
{
    // No other controller code needed for out-of-box support for GET/PUT/POST/PATCH/DELETE
    // Reference https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview for OData Query Structure
}
```

### Add Dependency Injection Entries

```
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddOData();
    services.AddControllers();
    services.AddDbContext<MyDbContext>( ... ); // Define your connection options here
    services.AddScoped(typeof(IODataHelperRepositoryAdapter<>), typeof(MyODataRepositoryAdapter<>))
    // ...
    
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.EnableDependencyInjection();
        endpoints.MapODataRoute("odata", "odata", GetEdmModel(), GetODataBatchHandler());
    });
    // ...
}

private static IEdmModel GetEdmModel()
{
    var oDataModelBuilder = ODataConventionModelBuilder;
    oDataModelBuilder.EntitySet<User>("Users");
    return oDataModelBuilder.GetEdmModel();
}

private static ODataBatchHandler GetODataBatchHandler()
{
    var oDataBatchHandler = new DefaultODataBatchHandler();
    return oDataBatchHandler;
}
```

### Browser Test
```
curl --get --verbose http://localhost:5000/odata/Users

> GET /odata/Resource HTTP/1.1
> Host: localhost:5000
> User-Agent: curl/7.55.1
> Accept: */*
>
< HTTP/1.1 202 Accepted
< Date: Mon, 29 Mar 2021 20:28:29 GMT
< Content-Type: application/json; odata.metadata=minimal; odata.streaming=true
< Server: Kestrel
< Transfer-Encoding: chunked
< OData-Version: 4.0
<
{"@odata.context":"http://localhost:5000/odata/$metadata#Users","value":[{"Id":1,"FirstName":"John","LastName":"Doe"}]}
```