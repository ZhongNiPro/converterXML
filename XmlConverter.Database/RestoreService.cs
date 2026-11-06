using System.Diagnostics;

namespace XmlConverter.Database;

public class RestoreService
{
    public async Task RestoreAsync(string sourceFilePath, string host, string user, string password, string dbName)
    {
        if (!File.Exists(sourceFilePath)) throw new FileNotFoundException("Файл бэкапа не найден.", sourceFilePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = "pg_restore.exe",
            Arguments = $"--host={host} --username={user} --dbname={dbName} --clean \"{sourceFilePath}\"",
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
            throw new Exception($"Ошибка восстановления: {error}");
        }
    }
}