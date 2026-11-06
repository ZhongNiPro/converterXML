using System.Diagnostics;

namespace XmlConverter.Database;

public class BackupService
{
    public async Task CreateAsync(string destinationFilePath, string host, string user, string password, string dbName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "pg_dump.exe",
            Arguments = $"--host={host} --username={user} --format=custom --file=\"{destinationFilePath}\" {dbName}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };
        startInfo.EnvironmentVariables["PGPASSWORD"] = password;

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"Ошибка бэкапа: {error}");
        }
    }
}
