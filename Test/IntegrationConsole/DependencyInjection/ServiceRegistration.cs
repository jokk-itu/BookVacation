using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TestConsole.DependencyInjection;

public static class ServiceRegistration
{
    public static ITypeRegistrar GetServices()
    {
        var services = new ServiceCollection();
        return new TypeRegistrar(services);
    }
}