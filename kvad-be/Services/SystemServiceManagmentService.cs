using System.Diagnostics;
using System.Text.Json.Nodes;

public class SystemServiceManagmentService
{

    public async Task<JsonArray> GetProcessList()
    {
        return await Task.Run(() =>
        {
            var processes = Process.GetProcesses();
            JsonArray jsonArray = new JsonArray();

            foreach (var process in processes)
            {
                JsonObject jsonObject = new JsonObject
                {
                    { "Name", process.ProcessName },
                    { "Id", process.Id },
                    { "Memory", process.WorkingSet64 }
                };
                jsonArray.Add(jsonObject);
            }

            return jsonArray;
        });
    }

    public async Task<JsonArray> GetServiceList(String? type, String? status)
    {
        JsonArray jsonArray = new JsonArray();

        var arguments = "list-units --no-legend";
        if (!string.IsNullOrEmpty(type))
        {
            arguments += $" --type={type}";
        }
        if (!string.IsNullOrEmpty(status))
        {
            arguments += $" --state={status}";
        }

        var processes = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        processes.Start();

        while (!processes.StandardOutput.EndOfStream)
        {
            var line = processes.StandardOutput.ReadLine();
            if (line == null)
            {
                continue;
            }
            var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("Name", split[0]);
            jsonObject.Add("Status", split[2]);
            jsonArray.Add(jsonObject);
        }

        await processes.WaitForExitAsync();

        return jsonArray;
    }

    public async Task<JsonObject> GetService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"show {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();

        JsonObject jsonObject = new JsonObject();

        foreach (var line in output.Split("\n"))
        {
            var split = line.Split("=");
            if (split.Length == 2)
            {
                jsonObject.Add(split[0], split[1]);
            }
        }

        return jsonObject;
    }

    public async Task<int> StartService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"start {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    public async Task<int> StopService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"stop {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    public async Task<int> RestartService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"restart {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    public async Task<int> EnableService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"enable {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    public async Task<int> DisableService(String name)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"disable {name}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

}