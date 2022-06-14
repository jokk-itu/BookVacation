using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace IntegrationConsole.DependencyInjection;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;
 
    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.BuildServiceProvider());
    }
    
    public void RegisterLazy(Type service, Func<object> factory)
    {
        _builder.AddSingleton(service, factory);
    }
 
    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }
 
    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }
}