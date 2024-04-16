using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Altairis.NetUtils.Backend;

public class QueueProcessor(ILogger<QueueProcessor> logger, IServiceProvider serviceProvider) : BackgroundService {
    private const int LOOP_DELAY = 1000;    // ms
    private const int MAX_TTL = 32;
    private const int BUFFER_LENGTH = 32;   // bytes
    private const int TIMEOUT = 1000;       // ms
    private byte[] pingBuffer = new byte[BUFFER_LENGTH];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        // Fill the ping buffer with random data
        Random.Shared.NextBytes(this.pingBuffer);

        // Run in almost infinite loop
        while (!stoppingToken.IsCancellationRequested) {
            // Get DB context from IoC/DI
            using var scope = serviceProvider.CreateScope();
            using var dc = scope.ServiceProvider.GetRequiredService<NetUtilsDbContext>();

            // Get next waiting job from queue
            var job = await dc.TraceJobs
                .Where(x => x.Status == TraceJobStatus.Waiting)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync(cancellationToken: stoppingToken);

            // Continue if no waiting job in queue
            if (job == null) {
                await Task.Delay(LOOP_DELAY, stoppingToken);
                continue;
            }

            // Set job status as being processed
            logger.LogInformation("Starting job {id} for host {host}.", job.Id, job.Host);
            job.Status = TraceJobStatus.Processing;
            job.DateStarted = DateTime.Now;
            await dc.SaveChangesAsync(stoppingToken);

            // Process job
            try {
                job.Result = await this.TraceHostAsync(job.Host, stoppingToken);
                job.Status = job.Result == null ? TraceJobStatus.Waiting : TraceJobStatus.Completed;
                logger.LogInformation("Completed job {id} for host {host}.", job.Id, job.Host);
            } catch (Exception ex) {
                job.Result = ex.Message;
                job.Status = TraceJobStatus.Error;
                logger.LogWarning(ex, "Error while processing job {id}.", job.Id);
            }
            job.DateCompleted = DateTime.Now;
            await dc.SaveChangesAsync(stoppingToken);
        }
    }

    private async Task<string?> TraceHostAsync(string host, CancellationToken stoppingToken) {
        var sb = new StringBuilder();

        // Prepare pinger with options
        using var ping = new Ping();
        var options = new PingOptions(ttl: 1, dontFragment: true);

        // Send pings
        for (var i = 0; i < MAX_TTL; i++) {
            // Exit if stopping
            if (stoppingToken.IsCancellationRequested) return null;

            // Send ping and process reply
            var reply = ping.Send(host, TIMEOUT, this.pingBuffer, options);
            if (reply.Status == IPStatus.TimedOut) {
                logger.LogDebug("TTL={ttl}: Timeout", options.Ttl);
                sb.AppendLine($"{options.Ttl,2}: {reply.Status}");
            } else {
                logger.LogDebug("TTL={ttl}: {status} from {hostName} in {time} ms", options.Ttl, reply.Status, reply.Address, reply.RoundtripTime);
                var hostName = await this.GetHostName(reply.Address.ToString(), stoppingToken);
                sb.AppendLine($"{options.Ttl,2}: {reply.Status} from {hostName} in {reply.RoundtripTime} ms");
                if (reply.Status == IPStatus.Success) break;
            }

            // Increment TTL for next ping
            options.Ttl++;
        }

        return sb.ToString();
    }

    private async Task<string> GetHostName(string ip, CancellationToken stoppingToken) {
        try {
            var r = await Dns.GetHostEntryAsync(ip, stoppingToken);
            logger.LogDebug("IP {ip} resolved to {host}.", ip, r.HostName);
            return $"{ip} [{r.HostName}]";
        } catch (SocketException) {
            logger.LogDebug("IP {ip} not resolved.", ip);
            return ip;
        }
    }

}
