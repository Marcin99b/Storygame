using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Cqrs;

public static class CqrsModule
{
    public static void RegisterCqrs(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, Dispatcher>();

        //todo if handlers doesn't use request specific data, then it could be changed to singletons
        //todo - decide when there will be any performance problems

        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}
