using BiddingService;
using BiddingService.Repositories;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using NLog;
using NLog.Web;
using BiddingService.Repositories.DBContext;

var logger = NLog.LogManager.Setup()
                            .LoadConfigurationFromAppSettings()
                            .GetCurrentClassLogger();

logger.Info($"Bidding worker service running at {DateTimeOffset.Now}");

#region Configuration

var vaultUrl = Environment.GetEnvironmentVariable("vaultUrl");

if (string.IsNullOrEmpty(vaultUrl)) //azure flow
{
// need ConnectionString, DatabaseName, jwtSecret, jwtIssuer, MqHost and LokiEndpoint in bicep env variables
}
else //compose flow
{
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

IAuthMethodInfo authMethod = new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000"); //undersøg om er korrekt

var vaultClientSettings = new VaultClientSettings(vaultUrl, authMethod)
{
Namespace = "",
MyHttpClientProviderFunc = handler => new HttpClient(httpClientHandler) { BaseAddress = new Uri(vaultUrl) }
};

IVaultClient vaultClient = new VaultClient(vaultClientSettings);

Secret<SecretData> vaultSecret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "Secrets", mountPoint: "secret");

Environment.SetEnvironmentVariable("LokiEndpoint", "http://loki:3100"); //compose
Environment.SetEnvironmentVariable("ConnectionString", vaultSecret.Data.Data["ConnectionString"].ToString());
Environment.SetEnvironmentVariable("DatabaseName", vaultSecret.Data.Data["DatabaseName"].ToString());
Environment.SetEnvironmentVariable("MqHost", vaultSecret.Data.Data["MqHost"].ToString());
}

#endregion   

try
{
    IHost host = Host.CreateDefaultBuilder(args)
                     .ConfigureServices(services => 
                     {
                         services.AddSingleton<MongoDBContext>();
                         services.AddSingleton<IBiddingRepository, BiddingRepository>();
                         services.AddHostedService<Worker>();
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
