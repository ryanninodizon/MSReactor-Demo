var builder = DistributedApplication.CreateBuilder(args);

var rabbitMQ = builder.AddRabbitMQ("messaging")
.WithManagementPlugin();

var senderApi = builder.AddProject<Projects.AspireRabbitMQ_Sender>("sender")
.WithReference(rabbitMQ);

builder.AddProject<Projects.AspireRabbitMQ_Receiver>("receiver")
.WithReference(rabbitMQ);

builder.AddNpmApp("angular", "../AspireRabbitMQ.Angular")
    .WithReference(senderApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddNpmApp("react", "../AspireRabbitMQ.React")
    .WithReference(senderApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();    

builder.Build().Run();
