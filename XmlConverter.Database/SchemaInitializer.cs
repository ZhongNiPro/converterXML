using Npgsql;

namespace XmlConverter.Database
{
    public class SchemaInitializer
    {
        private readonly string _connectionString;

        private const string CreateTablesSql = $@"
            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Registry} (
                {DatabaseConstants.Columns.ObjectId} BIGSERIAL PRIMARY KEY,
                {DatabaseConstants.Columns.ObjectType} TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Quarters} (
                {DatabaseConstants.Columns.QuarterId} BIGSERIAL PRIMARY KEY,
                {DatabaseConstants.Columns.QuarterNumber} TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Lands} (
                {DatabaseConstants.Columns.ObjectId} BIGINT PRIMARY KEY REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.CadastralNumber} TEXT NOT NULL,
                {DatabaseConstants.Columns.ExtractDate} DATE NOT NULL,
                {DatabaseConstants.Columns.SubType} TEXT,
                {DatabaseConstants.Columns.Category} TEXT,
                {DatabaseConstants.Columns.PermittedUse} TEXT,
                {DatabaseConstants.Columns.Area} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Inaccuracy} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Address} TEXT,
                {DatabaseConstants.Columns.Cost} NUMERIC(15, 2),
                {DatabaseConstants.Columns.SpecialNotes} TEXT,
                {DatabaseConstants.Columns.QuarterId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Quarters}({DatabaseConstants.Columns.QuarterId}) ON DELETE RESTRICT,
                CONSTRAINT uq_land_date UNIQUE ({DatabaseConstants.Columns.CadastralNumber}, {DatabaseConstants.Columns.ExtractDate})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.LandAscendants} (
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Lands}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.RelatedCadastralNumber} TEXT NOT NULL,
                PRIMARY KEY ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.RelatedCadastralNumber})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.LandDescendants} (
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Lands}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.RelatedCadastralNumber} TEXT NOT NULL,
                PRIMARY KEY ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.RelatedCadastralNumber})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.LandIncluded} (
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Lands}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.RelatedCadastralNumber} TEXT NOT NULL,
                PRIMARY KEY ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.RelatedCadastralNumber})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.LandParts} (
                {DatabaseConstants.Columns.ObjectId} BIGINT PRIMARY KEY REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.ParentObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Lands}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.PartNumber} INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.PropertyRights} (
                right_id BIGSERIAL PRIMARY KEY,
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.RecordNumber} INTEGER NOT NULL,
                {DatabaseConstants.Columns.RightType} TEXT NOT NULL,
                {DatabaseConstants.Columns.RightHolder} TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Builts} (
                {DatabaseConstants.Columns.ObjectId} BIGINT PRIMARY KEY REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.CadastralNumber} TEXT NOT NULL,
                {DatabaseConstants.Columns.ExtractDate} DATE NOT NULL,
                {DatabaseConstants.Columns.Purpose} TEXT,
                {DatabaseConstants.Columns.Area} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Address} TEXT,
                {DatabaseConstants.Columns.Cost} NUMERIC(15, 2),
                {DatabaseConstants.Columns.SpecialNotes} TEXT,
                {DatabaseConstants.Columns.Extension} NUMERIC(15, 2),
                {DatabaseConstants.Columns.BuiltUpArea} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Depth} NUMERIC(15, 2),
                {DatabaseConstants.Columns.OccurenceDepth} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Volume} NUMERIC(15, 2),
                {DatabaseConstants.Columns.Height} NUMERIC(15, 2),
                {DatabaseConstants.Columns.QuarterId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Quarters}({DatabaseConstants.Columns.QuarterId}) ON DELETE RESTRICT,
                CONSTRAINT uq_built_date UNIQUE ({DatabaseConstants.Columns.CadastralNumber}, {DatabaseConstants.Columns.ExtractDate})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Boundaries} (
                {DatabaseConstants.Columns.ObjectId} BIGINT PRIMARY KEY REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.RegNumbBorder} TEXT NOT NULL,
                {DatabaseConstants.Columns.ExtractDate} DATE NOT NULL,
                {DatabaseConstants.Columns.TypeBoundary} TEXT,
                {DatabaseConstants.Columns.TypeZone} TEXT,
                {DatabaseConstants.Columns.Description} TEXT,
                {DatabaseConstants.Columns.NameByDoc} TEXT,
                CONSTRAINT uq_boundary_date UNIQUE ({DatabaseConstants.Columns.RegNumbBorder}, {DatabaseConstants.Columns.ExtractDate})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.BoundaryQuarters} (
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Boundaries}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.QuarterId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Quarters}({DatabaseConstants.Columns.QuarterId}) ON DELETE CASCADE,
                PRIMARY KEY ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.QuarterId})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Blocks} (
                {DatabaseConstants.Columns.ObjectId} BIGINT PRIMARY KEY REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.QuarterId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Quarters}({DatabaseConstants.Columns.QuarterId}) ON DELETE RESTRICT,
                {DatabaseConstants.Columns.OrganRegistrRight} TEXT,
                {DatabaseConstants.Columns.ExtractDate} DATE NOT NULL,
                CONSTRAINT uq_block_quarter UNIQUE ({DatabaseConstants.Columns.QuarterId}, {DatabaseConstants.Columns.ExtractDate})
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Contours} (
                {DatabaseConstants.Columns.ContourId} BIGSERIAL PRIMARY KEY,
                {DatabaseConstants.Columns.ObjectId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Registry}({DatabaseConstants.Columns.ObjectId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.ContourNumber} INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {DatabaseConstants.Tables.Coordinates} (
                coordinate_id BIGSERIAL PRIMARY KEY,
                {DatabaseConstants.Columns.ContourId} BIGINT NOT NULL REFERENCES {DatabaseConstants.Tables.Contours}({DatabaseConstants.Columns.ContourId}) ON DELETE CASCADE,
                {DatabaseConstants.Columns.OrdNmb} INTEGER NOT NULL,
                {DatabaseConstants.Columns.X} DOUBLE PRECISION NOT NULL,
                {DatabaseConstants.Columns.Y} DOUBLE PRECISION NOT NULL
            );";

        public SchemaInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task InitializeAsync()
        {
            await using var databaseConnection = new NpgsqlConnection(_connectionString);
            await databaseConnection.OpenAsync();
            await using var command = new NpgsqlCommand(CreateTablesSql, databaseConnection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> VerifyTablesExistAsync(string[] expectedTables)
        {
            const string query = @"
        SELECT COUNT(table_name) 
        FROM information_schema.tables 
        WHERE table_schema = 'public' 
          AND table_name = ANY(@tables);";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("tables", expectedTables);

            var actualCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            return actualCount == expectedTables.Length;
        }
    }
}