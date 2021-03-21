namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public interface IConfigurationProviderChainBuilder
    {
        BaseConfigurationProvider Build();
    }
}
