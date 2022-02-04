var builder = WebApplication.CreateBuilder(args);

//ADD SERVICES
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//PIPELINE SETUP
var app = builder.Build();
app.MapReverseProxy();
app.Run();