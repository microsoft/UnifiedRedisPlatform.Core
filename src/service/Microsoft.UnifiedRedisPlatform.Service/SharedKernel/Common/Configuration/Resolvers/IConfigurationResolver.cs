namespace Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers
{
    public interface IConfigurationResolver<T> where T:BaseConfiguration
    {
        T Resolve();
    }
}
