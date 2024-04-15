var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var personApi = builder.AddProject<Projects.PersonData_API>("personapi")
    .WithReference(cache);

builder.Build().Run();
