using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkItems.Model;

namespace WorkItems.Utility;

public static class AdoConnectionUtility
{
    // Cross platform Token Cache:
    // https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache

    // App settings
    private const string Authority = "https://login.windows.net/common";
    private const string VisualStudioIdeClientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
    private const string AzureDevOpsScope = "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation";
    private const string TokenCacheFileName = "TokenCache.bin";

    // For Mac
    public const string KeyChainServiceName = "myapp_msal_service";
    public const string KeyChainAccountName = "myapp_msal_account";

    // For Linux
    private const string LinuxKeyRingSchema = "com.peterspada.wi.tokencache";
    private const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    private static readonly string LinuxKeyRingLabel = $"MSAL token cache for {Strings.AppInternalName} app.";
    private static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new("Version", "1");
    private static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new("ProductGroup", "MyApps");

    public static async Task<AdoConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        // Initialize the MSAL library by building a public client application
        IPublicClientApplication application = PublicClientApplicationBuilder
            .Create(AdoConnectionUtility.VisualStudioIdeClientId)
            .WithAuthority(AdoConnectionUtility.Authority)
            .WithDefaultRedirectUri()
            .Build();

        StorageCreationProperties storageProperties = new StorageCreationPropertiesBuilder(AdoConnectionUtility.TokenCacheFileName, FileUtility.UserRootDirectory)
            .WithLinuxKeyring(
                AdoConnectionUtility.LinuxKeyRingSchema,
                AdoConnectionUtility.LinuxKeyRingCollection,
                AdoConnectionUtility.LinuxKeyRingLabel,
                AdoConnectionUtility.LinuxKeyRingAttr1,
                AdoConnectionUtility.LinuxKeyRingAttr2)
            .WithMacKeyChain(
                AdoConnectionUtility.KeyChainServiceName,
                AdoConnectionUtility.KeyChainAccountName)
            .Build();

        try
        {
            MsalCacheHelper cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            cacheHelper.RegisterCache(application.UserTokenCache);
        }
        catch
        {
            // This shouldn't fail on Windows
        }

        string[] scopes = new string[] { AdoConnectionUtility.AzureDevOpsScope };
        AuthenticationResult authentication;
        try
        {
            IEnumerable<IAccount> accounts = await application.GetAccountsAsync();
            authentication = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync(cancellationToken);
        }
        catch (MsalUiRequiredException ex)
        {
            authentication = await application.AcquireTokenInteractive(scopes).WithClaims(ex.Claims).ExecuteAsync(cancellationToken);
        }

        return new AdoConnection()
        {
            AccessToken = authentication.AccessToken,
            UserName = authentication.Account.Username,
        };
    }
}
