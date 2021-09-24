using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Models;

public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private Timer _timer;

    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        var count = Interlocked.Increment(ref executionCount);

        // _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        var data = new DataPacket();
        SendMessage(JsonSerializer.Serialize(data));
    }

    private void SendMessage(string message)
    {
        var data = Encoding.Default.GetBytes(message);
        using (var udpClient = new UdpClient(AddressFamily.InterNetwork))
        {
            var address = IPAddress.Parse("224.100.0.1");
            var ipEndPoint = new IPEndPoint(address, 8088);
            udpClient.JoinMulticastGroup(address);
            udpClient.Send(data, data.Length, ipEndPoint);
            udpClient.Close();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
