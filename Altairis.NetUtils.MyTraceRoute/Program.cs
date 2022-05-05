using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

const int TTL = 32;
const int BUFFER_LENGTH = 32;   // bytes
const int TIMEOUT = 1000;       // ms

// Check command line argument
if (args.Length != 1) {
    Console.WriteLine("USAGE: mytraceroute <host>");
    return 1;
}

// Prepare pinger with options
using var ping = new Ping();
var options = new PingOptions {
    Ttl = 1,
    DontFragment = true
};

// Prepare buffer
var buffer = new byte[BUFFER_LENGTH];
Random.Shared.NextBytes(buffer);

// Send pings
for (int i = 0; i < TTL; i++) {
    var reply = ping.Send(args[0], TIMEOUT, buffer, options);
    if (reply.Status == IPStatus.TimedOut) {
        Console.WriteLine($"{i + 1,2}: {reply.Status}");
    } else {
        Console.WriteLine($"{i + 1,2}: {reply.Status} from {GetHostName(reply.Address)} in {reply.RoundtripTime} ms");
        if (reply.Status == IPStatus.Success) break;
    }
    options.Ttl++;
}

return 0;

static string GetHostName(IPAddress ip) {
    try {
        var r = Dns.GetHostEntry(ip);
        return $"{ip} [{r.HostName}]";
    } catch (SocketException) {
        return ip.ToString();
    }
}