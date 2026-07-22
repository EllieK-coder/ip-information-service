using Autofac;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using IPService.Application.Decorators;

namespace IPService.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var configuration = MediatRConfigurationBuilder
            .Create(licenseKey: null, typeof(ApplicationModule).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        builder.RegisterMediatR(configuration);

        builder.RegisterGenericDecorator(
            typeof(CachingRequestHandler<,>),
            typeof(IRequestHandler<,>));

        builder.RegisterGenericDecorator(
           typeof(LoggingRequestHandler<,>),
           typeof(IRequestHandler<,>));
    }
}