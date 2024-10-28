//Service gets system information

using System.Diagnostics;
using System.Text.Json.Nodes;

public class SystemInfoService
{
    public Task<JsonArray> GetCPUInfo()
    {
        var jsonString = File.ReadAllLines("/proc/cpuinfo");
        JsonArray jsonArray = new JsonArray();
        JsonObject? currentProcessor = null;

        foreach (var line in jsonString)
        {
            if (line == "")
            {
                jsonArray.Add(currentProcessor);
                currentProcessor = null;
            }
            else
            {
                var split = line.Split(":");
                if (currentProcessor == null)
                {
                    currentProcessor = new JsonObject();
                }

                var key = split[0].Trim();

                if (new[] { "flags", "bugs" }.Contains(key))
                {
                    var flags = split[1].Trim().Split(" ");
                    JsonArray flagsArray = new JsonArray();
                    foreach (var flag in flags)
                    {
                        flagsArray.Add(flag);
                    }
                    currentProcessor.Add(key, flagsArray);
                }
                else
                {
                    currentProcessor.Add(key, split[1].Trim() == "" ? null : split[1].Trim());
                }
            }
        }


        return Task.FromResult(jsonArray);
    }

    public Task<JsonObject> GetMemoryInfo()
    {
        var jsonString = File.ReadAllLines("/proc/meminfo");
        JsonObject jsonObject = new JsonObject();

        foreach (var line in jsonString)
        {
            var split = line.Split(":");
            var key = split[0].Trim();
            var valueParts = split[1].Trim().Split(" ");
            JsonObject value = new JsonObject
            {
                { "value", valueParts.Length > 0 ? valueParts[0] : null },
                { "unit", valueParts.Length > 1 ? valueParts[1] : null }
            };
            jsonObject.Add(key, value);
        }

        return Task.FromResult(jsonObject);
    }

    public Task<JsonNode> GetDiskInfo()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"lsblk --json\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var jsonObject = JsonNode.Parse(output) ?? new JsonObject();

        return Task.FromResult<JsonNode>(jsonObject);
    }

    public Task<JsonNode> GetNetworkInfo()
    {
        //ip -j -s addr

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"ip -j -s addr\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var jsonObject = JsonNode.Parse(output) ?? new JsonObject();

        return Task.FromResult<JsonNode>(jsonObject);
    }

    public Task<JsonObject> GetSystemVersion()
    {
        var jsonString = File.ReadAllText("/proc/version");

        var jsonObject = new JsonObject
        {
            { "version", jsonString }
        };

        return Task.FromResult(jsonObject);

    }

    public Task<JsonObject> GetUptimeInfo()
    {
        var jsonString = File.ReadAllText("/proc/uptime");

        var uptime = double.Parse(jsonString.Split(" ")[0]);
        var uptimeString = TimeSpan.FromSeconds(uptime).ToString();

        var idleTime = double.Parse(jsonString.Split(" ")[1]);
        var idleTimeString = TimeSpan.FromSeconds(idleTime).ToString();

        var jsonObject = new JsonObject
        {
            { "uptime", uptimeString },
            { "idleTime", idleTimeString }
        };

        return Task.FromResult(jsonObject);
    }

    public Task<JsonObject> GetOperatingSystemInfo()
    {
        var jsonString = File.ReadAllText("/etc/os-release");
        var jsonObject = new JsonObject();

        foreach (var line in jsonString.Split("\n"))
        {
            var split = line.Split("=");
            if (split.Length == 2)
            {
                jsonObject.Add(split[0], split[1].Trim('"'));
            }
        }

        return Task.FromResult(jsonObject);
    }
    public Task<JsonObject> GetMemoryUsage()
    {
        var jsonString = File.ReadAllLines("/proc/meminfo");
        JsonObject jsonObject = new JsonObject();

        foreach (var line in jsonString)
        {
            var split = line.Split(":");
            var key = split[0].Trim();
            var valueParts = split[1].Trim().Split(" ");
            JsonObject value = new JsonObject
            {
                { "value", valueParts.Length > 0 ? valueParts[0] : null },
                { "unit", valueParts.Length > 1 ? valueParts[1] : null }
            };
            jsonObject.Add(key, value);
        }

        return Task.FromResult(jsonObject);
    }

    public Task<JsonObject> GetCPULoad()
    {
        var jsonString = File.ReadAllText("/proc/loadavg");
        var jsonObject = new JsonObject();
        var split = jsonString.Split(" ");
        jsonObject.Add("1min", split[0]);
        jsonObject.Add("5min", split[1]);
        jsonObject.Add("15min", split[2]);
        jsonObject.Add("runningProcesses", split[3]);
        jsonObject.Add("totalProcesses", split[4]);

        return Task.FromResult(jsonObject);
    }
}

