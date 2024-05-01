var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var mssqlserver = builder.AddSqlServer("mssqlserver");
var companyDb = mssqlserver.AddDatabase("companydb");

var personapi = builder.AddProject<Projects.PersonData_API>("personapi")
    .WithReference(cache)
    .WithEnvironment("ConnectionStrings__AwcDb", "Server=tcp:localhost,1433;Database=AdventureWorks_Test;User Id=sa;Password=Info99Gum;TrustServerCertificate=True"); 

var companyapi = builder.AddProject<Projects.Company_API>("companyapi")
    .WithReference(cache)
    .WithReference(companyDb);

builder.AddProject<Projects.web_Bff>("apigateway")
    .WithReference(personapi)
    .WithReference(companyapi);

builder.Build().Run();
