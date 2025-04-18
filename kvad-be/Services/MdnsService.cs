using Zeroconf;

public class MdnsService
{

    private readonly MdnsDiscoveryService _mdnsDiscoveryService;

    public MdnsService(MdnsDiscoveryService mdnsDiscoveryService)
    {
        _mdnsDiscoveryService = mdnsDiscoveryService;
    }
    public IReadOnlyList<IZeroconfHost> ListMdnsDevices()
    {
        return [.. _mdnsDiscoveryService.discoveredDevices.Values];
    }
}