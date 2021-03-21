using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.UnifiedPlatform.Service.Common.Secrets;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using Microsoft.UnifiedPlatform.Service.Secrets.Wrapper;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;

/// <summary>
/// Provides application secrets from Azure Key Vault
/// </summary>
public class KeyVaultProvider: ISecretsProvider
{
    public const string KEY_VAULT_URI_FORMAT = "https://{0}.vault.azure.net";

    private readonly IKeyVaultClientWrapper _keyVaultClientWrapper;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Constructs the Key Vault provider from configuration
    /// </summary>
    /// <param name="configuration" cref="KeyvaultConfiguration">Configuration for connecting to Azure Key Vault</param>
    /// <param name="cacheService" cref="ICacheService">Service for caching data</param>
    public KeyVaultProvider(string keyVaultName, ICacheService cacheService)
    {
        var keyVaultUri = string.Format(KEY_VAULT_URI_FORMAT, keyVaultName);
        _cacheService = cacheService;
        var tokenProvider = new AzureServiceTokenProvider();
        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
        _keyVaultClientWrapper = new KeyVaultClientWrapper(keyVaultUri, keyVaultClient);
    }

    /// <summary>
    /// Constructs the key vault provider from a client
    /// </summary>
    /// <param name="configuration" cref="KeyvaultConfiguration">Configuration for connecting to Azure Key Vault</param>
    /// <param name="wrapper" cref="IKeyVaultClientWrapper">Wrapped Client for interfacing with Azure KeyVault</param>
    /// <param name="cacheService" cref="ICacheService">Service for caching data</param>
    /// <remarks>Used for unit testing</remarks>
    public KeyVaultProvider(IKeyVaultClientWrapper wrapper, ICacheService cacheService)
    {
        _keyVaultClientWrapper = wrapper;
        _cacheService = cacheService;
    }

    /// <summary>
    /// To get a secret value by key
    /// </summary>
    /// <param name="key" cref="string">Key name</param>
    /// <returns cref="string">Secret value</returns>
    public async Task<string> GetSecret(string key)
    {
        var cachedSecret = await _cacheService.Get<string>(key);
        if (!(string.IsNullOrEmpty(cachedSecret)) && !(string.IsNullOrWhiteSpace(cachedSecret)))
            return cachedSecret;

        try
        {
            var secretBundle = await _keyVaultClientWrapper.GetSecretAsync(key);
            var secret = secretBundle.Value;
            await _cacheService.Set(key, secret);
            return secret;

        }
        catch (KeyVaultErrorException exception)
        {
            if (exception.Response.StatusCode == HttpStatusCode.NotFound)
                throw new SecretNotFoundException(key, innerException: exception);
            throw;
        }
    }
}
