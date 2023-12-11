using BiddingService;
using BiddingService.Repositories;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup()
                            .LoadConfigurationFromAppSettings()
                            .GetCurrentClassLogger();

logger.Info($"Bidding worker service running at {DateTimeOffset.Now}");

#region Vault

//var EndPoint = "https://vault";
var EndPoint = "https://localhost:8251";
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

IAuthMethodInfo authMethod = new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");

var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
{
    Namespace = "",
    MyHttpClientProviderFunc = handler => new HttpClient(httpClientHandler) { BaseAddress = new Uri(EndPoint) }
};

IVaultClient vaultClient = new VaultClient(vaultClientSettings);

Secret<SecretData> vaultSecret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "Secrets", mountPoint: "secret");

#endregion   

try
{
    IHost host = Host.CreateDefaultBuilder(args)
                     .ConfigureServices(services => 
                     { 
                         services.AddHostedService<Worker>(); 
                         services.AddScoped<IBiddingRepository, BiddingRepository>();
                         services.AddSingleton<Secret<SecretData>>(vaultSecret);
                     })
                     .UseNLog()
                     .Build();

    host.Run();
}
catch (Exception ex)
{
    logger.Error(ex, $"Bidding worker service encountered a fatal error, shutting down at {DateTimeOffset.Now}");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
