using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zeroconf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class MdnsDiscoveryService : BackgroundService
{
    private readonly ILogger<MdnsDiscoveryService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Discovery interval

    public MdnsDiscoveryService(ILogger<MdnsDiscoveryService> logger)
    {
        _logger = logger;
    }

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
}
