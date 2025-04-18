using System.Collections.Concurrent;
using Zeroconf;

public class MdnsService
{
    public ConcurrentDictionary<string, IZeroconfHost> discoveredDevices = new();

    public IReadOnlyList<IZeroconfHost> ListMdnsDevices()
    {
        return [.. discoveredDevices.Values];
    }
}