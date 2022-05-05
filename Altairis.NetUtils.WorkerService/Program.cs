using Altairis.NetUtils.Backend;
using Microsoft.EntityFrameworkCore;

// Register services
var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureServices(services => {
    services.AddDbContext<NetUtilsDbContext>(options => {
        options.UseSqlServer("SERVER=.\\SqlExpress;TRUSTED_CONNECTION=yes;DATABASE=TraceServer");
    });
    services.AddHostedService<QueueProcessor>();
});

// Use appropriate daemon type
if (Environment.OSVersion.Platform == PlatformID.Unix) {
    hostBuilder.UseSystemd();
} else {
    hostBuilder.UseWindowsService();
}

// Run host
var host = hostBuilder.Build();
await host.RunAsync();