using Zeroconf;

public class MdnsDiscoveryService(ILogger<MdnsDiscoveryService> logger, MdnsService mdnsService) : BackgroundService
{
    private readonly ILogger<MdnsDiscoveryService> _logger = logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Discovery interval
    private readonly MdnsService _mdnsService = mdnsService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("mDNS Discovery Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1) Query the DNS-SD meta-service to learn all advertised service types
                var metaService = "_services._dns-sd._udp.local.";
                IReadOnlyList<IZeroconfHost> metaResults = Array.Empty<IZeroconfHost>();
                try
                {
                    metaResults = await ZeroconfResolver.ResolveAsync(metaService);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to query DNS-SD meta-service ({Meta})", metaService);
                }

                var serviceTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // The meta-results contain PTR records for each service type under the meta service.
                foreach (var host in metaResults)
                {
                    foreach (var kv in host.Services)
                    {
                        // The dictionary key is typically the service type (e.g. "_http._tcp.local.")
                        if (!string.IsNullOrWhiteSpace(kv.Key))
                            serviceTypes.Add(kv.Key);

                        // Some implementations may expose the service name on the value
                        try
                        {
                            var svc = kv.Value;
                            if (svc != null)
                            {
                                if (!string.IsNullOrWhiteSpace(svc.Name))
                                    serviceTypes.Add(svc.Name);
                            }
                        }
                        catch { /* ignore */ }
                    }
                }

                // Fallback: if meta discovery returned nothing, probe a common set to ensure at least HTTP is discovered
                if (serviceTypes.Count == 0)
                {
                    serviceTypes.Add("_http._tcp.local.");
                }

                _logger.LogDebug("Discovered {Count} service types via mDNS", serviceTypes.Count);

                // 2) Resolve each discovered service type and record discovered hosts
                foreach (var serviceType in serviceTypes)
                {
                    IReadOnlyList<IZeroconfHost> results = Array.Empty<IZeroconfHost>();
                    try
                    {
                        results = await ZeroconfResolver.ResolveAsync(serviceType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Failed to resolve service type {ServiceType}", serviceType);
                        continue;
                    }

                    foreach (var host in results)
                    {
                        try
                        {
                            _mdnsService.discoveredDevices[host.DisplayName] = host; // Store discovered devices
                            _logger.LogDebug("mDNS discovered {Host} for service {Service} (IPs: {IPs})",
                                host.DisplayName, serviceType, string.Join(',', host.IPAddresses ?? Array.Empty<string>()));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Failed to store mdns host {Host}", host.DisplayName);
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
