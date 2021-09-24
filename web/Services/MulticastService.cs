using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

class MulticastService : IHostedService
{
    UdpClient udpClient;
    Dictionary<string, DateTime> _clients = new Dictionary<string, DateTime>();
    public MulticastService()
    {
        udpClient = new UdpClient(8088);
        udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.0.1"), 50);
    }
    public void Receive()
    {
        while (true)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var data = udpClient.Receive(ref ipEndPoint);

            var Message = Encoding.Default.GetString(data);

            var json = JsonSerializer.Deserialize<DataPacket>(Message);
            
            Models.PersistedData.Clients[json.Hostname] = DateTime.Now;
            // _clients[json.Hostname] = DateTime.Now;

            System.Console.WriteLine(json.Hostname);
        }
    }

    public class DataPacket {
        public string Hostname {get;set;}
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var receiveThread = new Thread(Receive);
        receiveThread.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        udpClient.Close();
        return Task.CompletedTask;
    }
}