var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var personapi = builder.AddProject<Projects.PersonData_API>("personapi")
    .WithReference(cache)
    .WithEnvironment("ConnectionStrings__AwcDb", "Server=tcp:localhost,1433;Database=AdventureWorks_Test;User Id=sa;Password=Info99Gum;TrustServerCertificate=True"); 

var apigateway = builder.AddProject<Projects.web_Bff>("apigateway")
    .WithReference(personapi);

builder.Build().Run();
