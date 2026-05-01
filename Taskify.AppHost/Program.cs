using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var taskifyDb = postgres.AddDatabase("taskifydb");

var api = builder.AddProject<Projects.Taskify_Api>("api", options =>
{
    options.LaunchProfileName = "http";
})
    .WithReference(taskifyDb)
    .WaitFor(taskifyDb);

builder.AddProject<Projects.Taskify_Web>("web", options =>
{
    options.LaunchProfileName = "http";
})
    .WithEnvironment("TaskifyApiBaseUrl", api.GetEndpoint("http"))
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
