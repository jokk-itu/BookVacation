using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog((context, serviceProvider, config) =>
                {
                    config
                        .WriteTo.Seq("http://localhost:5341")
                        .Enrich.FromLogContext()
                        .MinimumLevel.Override("Api", LogEventLevel.Information)
                        .MinimumLevel.Warning();
                });
        }
    }
}