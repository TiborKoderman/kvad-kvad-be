using Zeroconf;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

public class MdnsDiscoveryService(ILogger<MdnsDiscoveryService> logger) : BackgroundService
{
    private readonly ILogger<MdnsDiscoveryService> _logger = logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Discovery interval
    private readonly ConcurrentDictionary<string, IZeroconfHost> _discoveredDevices = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("mDNS Discovery Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var servicesToQuery = new[] { "_http._tcp.local." }; // add more as needed
                foreach (var serviceType in servicesToQuery)
                {
                    IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync(serviceType);

                    foreach (var host in results)
                    {
                        _discoveredDevices[host.DisplayName] = host; // Store discovered devices
                        _logger.LogInformation("Discovered {Host} ({IP})", host.DisplayName, host.IPAddress);
                        foreach (var service in host.Services)
                        {
                            if (service.Value.Properties != null)
                            {
                                foreach (var txt in service.Value.Properties)
                                {
                                    _logger.LogInformation("TXT: {Key} = {Value}", txt.Keys, txt.Values);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during mDNS discovery");
            }
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("mDNS Discovery Service stopping.");
    }

    public IReadOnlyList<IZeroconfHost> ListMdnsDevices()
    {
        return [.. _discoveredDevices.Values];
    }
}
