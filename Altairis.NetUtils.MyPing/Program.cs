using System.Net.NetworkInformation;

const int TTL = 32;
const int COUNT = 10;
const int BUFFER_LENGTH = 32;   // bytes
const int TIMEOUT = 1000;       // ms

// Check command line argument
if (args.Length != 1) {
    Console.WriteLine("USAGE: myping <host>");
    return 1;
}

// Prepare pinger with options
using var ping = new Ping();
var options = new PingOptions {
    Ttl = TTL,
    DontFragment = true
};

// Prepare buffer
var buffer = new byte[BUFFER_LENGTH];
Random.Shared.NextBytes(buffer);

// Send pings
for (var i = 0; i < COUNT; i++) {
    var reply = ping.Send(args[0], TIMEOUT, buffer, options);
    if (reply.Status == IPStatus.TimedOut) {
        Console.WriteLine($"{i + 1,2}: {reply.Status}");
    } else {
        Console.WriteLine($"{i + 1,2}: {reply.Status} from {reply.Address} in {reply.RoundtripTime} ms");
    }
}

return 0;