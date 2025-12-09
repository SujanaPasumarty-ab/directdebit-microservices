var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DirectDebit_Subscription_Cancellation_Service>("directdebit-subscription-cancellation-service");

builder.Build().Run();
