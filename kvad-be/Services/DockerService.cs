using System.Diagnostics;
using System.Text.Json.Nodes;

public class DockerService {
    
    public Task<JsonArray> listDockerContainers() {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"docker inspect $(docker ps -aq) --format '{{json .}}'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        // Parse output into JSON Array
        var jsonArray = new JsonArray();
        var jsonLines = output.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in jsonLines)
        {
            var jsonObject = JsonNode.Parse(line)?.AsObject();
            if (jsonObject != null)
            {
                jsonArray.Add(jsonObject);
            }
        }

        return Task.FromResult(jsonArray);
    }

    public Task<string> getContainerId(String containerName) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker ps -qf name={containerName}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return Task.FromResult(output.Trim());
    }

    public Task<JsonNode> getContainerInfo(String containerId) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker inspect {containerId} --format '{{json .}}'\"",
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

    public Task<JsonNode> getContainerLogs(String containerId) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker logs {containerId}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var jsonObject = new JsonObject
        {
            { "logs", output }
        };

        return Task.FromResult<JsonNode>(jsonObject);
    }

    public Task startContainer(String containerId) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker start {containerId}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Failed to start container");
        }

        return Task.CompletedTask;
    }

    public Task<int> stopContainer(String containerId) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker stop {containerId}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        return Task.FromResult(process.ExitCode);
    }

    public Task<int> restartContainer(String containerId) {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"docker restart {containerId}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        return Task.FromResult(process.ExitCode);
    }


}