using Autofac;
using IPService.Application.Interfaces;
using IPService.Infrastructure.Persistence.Commands;
using IPService.Infrastructure.Persistence.Queries;

namespace IPService.Infrastructure;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GetIpAddressPersistence>()
            .As<IGetIpAddress>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SaveIpAddressPersistence>()
            .As<ISaveIpAddress>()
            .InstancePerLifetimeScope();

        builder.RegisterType<EnsureCountryPersistence>()
            .As<IEnsureCountry>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetIpBatchPersistence>()
            .As<IGetIpBatchPersistence>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateIpInfoPersistence>()
            .As<IUpdateIpInfoPersistence>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SaveJobHistoryPersistence>()
        .As<ISaveJobHistory>()
        .InstancePerLifetimeScope();
    }
}