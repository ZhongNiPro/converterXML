namespace XmlConverter.Database;

public class DatabaseManager
{
    private SchemaInitializer _schemaInitializer;
    private readonly BackupService _backupService;
    private readonly RestoreService _restoreService;

    public DatabaseManager(string connectionString)
    {
        _schemaInitializer = new SchemaInitializer(connectionString);
        _backupService = new BackupService();
        _restoreService = new RestoreService();
    }

    public async Task InitializeDatabaseAsync() => await _schemaInitializer.InitializeAsync();

    public async Task BackupDatabaseAsync(string path, string host, string user, string pass, string db)
        => await _backupService.CreateAsync(path, host, user, pass, db);

    public async Task RestoreDatabaseAsync(string path, string host, string user, string pass, string db)
        => await _restoreService.RestoreAsync(path, host, user, pass, db);

    public async Task<bool> CheckAndInitializeSchemaAsync()
    {
        string[] requiredTables = new string[]
        {
        DatabaseConstants.Tables.Registry,
        DatabaseConstants.Tables.Quarters,
        DatabaseConstants.Tables.Lands,
        DatabaseConstants.Tables.LandAscendants,
        DatabaseConstants.Tables.LandDescendants,
        DatabaseConstants.Tables.LandIncluded,
        DatabaseConstants.Tables.LandParts,
        DatabaseConstants.Tables.PropertyRights,
        DatabaseConstants.Tables.Builts,
        DatabaseConstants.Tables.Boundaries,
        DatabaseConstants.Tables.BoundaryQuarters,
        DatabaseConstants.Tables.Blocks,
        DatabaseConstants.Tables.Contours,
        DatabaseConstants.Tables.Coordinates
    };

        bool isSchemaValid = await _schemaInitializer.VerifyTablesExistAsync(requiredTables);

        if (isSchemaValid == false)
        {
            await InitializeDatabaseAsync();
        }

        return true;
    }

    public void UpdateConnectionString(string newConnectionString)
    {
        _schemaInitializer = new SchemaInitializer(newConnectionString);
    }
}