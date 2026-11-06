using Npgsql;
using XmlConverter.Data;

namespace XmlConverter.Database
{
    public partial class CadObjectRepository
    {
        private static readonly string SummaryQuery = $@"
    SELECT 
        {DatabaseConstants.Columns.ObjectId}, 
        {DatabaseConstants.Columns.CadastralNumber}, 
        '{DatabaseConstants.Discriminators.Land}' AS {DatabaseConstants.Columns.ObjectType}, 
        {DatabaseConstants.Columns.ExtractDate}
    FROM {DatabaseConstants.Tables.Lands}
    
    UNION ALL
    
    SELECT 
        {DatabaseConstants.Columns.ObjectId}, 
        {DatabaseConstants.Columns.CadastralNumber}, 
        '{DatabaseConstants.Discriminators.Built}' AS {DatabaseConstants.Columns.ObjectType}, 
        {DatabaseConstants.Columns.ExtractDate}
    FROM {DatabaseConstants.Tables.Builts}
    
    UNION ALL
    
    SELECT 
        {DatabaseConstants.Columns.ObjectId}, 
        {DatabaseConstants.Columns.RegNumbBorder} AS {DatabaseConstants.Columns.CadastralNumber}, 
        '{DatabaseConstants.Discriminators.Boundary}' AS {DatabaseConstants.Columns.ObjectType}, 
        {DatabaseConstants.Columns.ExtractDate}
    FROM {DatabaseConstants.Tables.Boundaries}
    
    ORDER BY {DatabaseConstants.Columns.ExtractDate} DESC;";

        private const string SearchParameterName = "@searchPattern";

        private static readonly string SearchQuery = $@"
    SELECT 
        sub.object_id, 
        sub.cadastral_number, 
        sub.object_type, 
        sub.extract_date
    FROM (
        {SummaryQuery}
    ) AS sub
    WHERE sub.cadastral_number LIKE {SearchParameterName};";

        //    private static readonly string DetermineTypeQuery = $@"
        //SELECT {DatabaseConstants.Columns.ObjectType} 
        //FROM {DatabaseConstants.Tables.Registry} 
        //WHERE {DatabaseConstants.Columns.ObjectId} = @objectId;";

        public async Task<List<CadObjectSummary>> GetObjectsSummaryAsync()
        {
            List<CadObjectSummary> objectList = new List<CadObjectSummary>();

            await using NpgsqlConnection databaseConnection = new NpgsqlConnection(_connectionString);
            await databaseConnection.OpenAsync();

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(SummaryQuery, databaseConnection);
            await using NpgsqlDataReader sqlReader = await sqlCommand.ExecuteReaderAsync();

            while (await sqlReader.ReadAsync())
            {
                CadObjectSummary summary = new CadObjectSummary
                {
                    ObjectId = sqlReader.GetInt64(0),
                    CadastralNumber = sqlReader.GetString(1) ?? string.Empty,
                    ObjectType = sqlReader.GetString(2) ?? string.Empty,
                    ExtractDate = sqlReader.GetDateTime(3)
                };

                objectList.Add(summary);
            }

            return objectList;
        }

        public async Task<List<CadObjectSummary>> SearchObjectsAsync(string cadastralNumberPattern)
        {
            if (string.IsNullOrWhiteSpace(cadastralNumberPattern))
            {
                return await GetObjectsSummaryAsync();
            }

            List<CadObjectSummary> filteredList = new List<CadObjectSummary>();

            await using NpgsqlConnection databaseConnection = new NpgsqlConnection(_connectionString);
            await databaseConnection.OpenAsync();

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(SearchQuery, databaseConnection);

            string formattedPattern = cadastralNumberPattern.Trim() + "%";
            sqlCommand.Parameters.AddWithValue(SearchParameterName, formattedPattern);

            await using NpgsqlDataReader sqlReader = await sqlCommand.ExecuteReaderAsync();

            while (await sqlReader.ReadAsync())
            {
                CadObjectSummary summary = new CadObjectSummary
                {
                    ObjectId = sqlReader.GetInt64(0),
                    CadastralNumber = sqlReader.GetString(1) ?? string.Empty,
                    ObjectType = sqlReader.GetString(2) ?? string.Empty,
                    ExtractDate = sqlReader.GetDateTime(3)
                };

                filteredList.Add(summary);
            }

            return filteredList;
        }

        public async Task<ICadastralObject?> GetFullObjectAsync(long objectId)
        {
            await using NpgsqlConnection databaseConnection = new NpgsqlConnection(_connectionString);
            await databaseConnection.OpenAsync();

            string determineTypeQuery = $@"
        SELECT {DatabaseConstants.Columns.ObjectType} 
        FROM {DatabaseConstants.Tables.Registry} 
        WHERE {DatabaseConstants.Columns.ObjectId} = @objectId;";

            string objectType = string.Empty;

            await using (NpgsqlCommand typeCommand = new NpgsqlCommand(determineTypeQuery, databaseConnection))
            {
                typeCommand.Parameters.AddWithValue("@objectId", objectId);
                object? result = await typeCommand.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                {
                    return null;
                }

                objectType = result.ToString() ?? string.Empty;
            }

            return objectType switch
            {
                DatabaseConstants.Discriminators.Land => await ReadLandSemanticAsync(databaseConnection, objectId),
                DatabaseConstants.Discriminators.Built => await ReadBuiltSemanticAsync(databaseConnection, objectId),
                DatabaseConstants.Discriminators.Boundary => await ReadBoundarySemanticAsync(databaseConnection, objectId),
                DatabaseConstants.Discriminators.Block => await ReadBlockSemanticAsync(databaseConnection, objectId),
                _ => null
            };
        }

        private async Task<CadObjectLand?> ReadLandSemanticAsync(NpgsqlConnection connection, long objectId)
        {
            string landQuery = $@"
        SELECT 
            land.{DatabaseConstants.Columns.ExtractDate},
            land.{DatabaseConstants.Columns.CadastralNumber},
            land.{DatabaseConstants.Columns.SubType},
            land.{DatabaseConstants.Columns.Category},
            land.{DatabaseConstants.Columns.PermittedUse},
            land.{DatabaseConstants.Columns.Area},
            land.{DatabaseConstants.Columns.Inaccuracy},
            land.{DatabaseConstants.Columns.Address},
            land.{DatabaseConstants.Columns.Cost},
            land.{DatabaseConstants.Columns.SpecialNotes},
            quarter.{DatabaseConstants.Columns.QuarterNumber}
        FROM {DatabaseConstants.Tables.Lands} AS land
        JOIN {DatabaseConstants.Tables.Quarters} AS quarter ON land.{DatabaseConstants.Columns.QuarterId} = quarter.{DatabaseConstants.Columns.QuarterId}
        WHERE land.{DatabaseConstants.Columns.ObjectId} = @objectId;";

            CadObjectLand? finalLand = null;

            await using (NpgsqlCommand command = new NpgsqlCommand(landQuery, connection))
            {
                command.Parameters.AddWithValue("@objectId", objectId);

                await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        finalLand = new CadObjectLand(
                            DateOnly.FromDateTime(reader.GetDateTime(0)),
                            reader.GetString(1),
                            reader.GetString(10),
                            DatabaseConstants.Discriminators.Land,
                            reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            reader.IsDBNull(5) ? 0.0 : reader.GetDouble(5),
                            reader.IsDBNull(6) ? 0.0 : reader.GetDouble(6),
                            reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            reader.IsDBNull(8) ? 0.0m : reader.GetDecimal(8),
                            reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                        );
                    }
                }
            }

            if (finalLand != null)
            {
                if (finalLand.AscendantCadNumbers != null)
                {
                    await FillRelatedNumbersAsync(connection, objectId, DatabaseConstants.Tables.LandAscendants, finalLand.AscendantCadNumbers);
                }

                if (finalLand.DescendantCadNumbers != null)
                {
                    await FillRelatedNumbersAsync(connection, objectId, DatabaseConstants.Tables.LandDescendants, finalLand.DescendantCadNumbers);
                }

                if (finalLand.IncludedCadNumbers != null)
                {
                    await FillRelatedNumbersAsync(connection, objectId, DatabaseConstants.Tables.LandIncluded, finalLand.IncludedCadNumbers);
                }

                if (finalLand.RightRecords != null)
                {
                    await FillRightRecordsAsync(connection, objectId, finalLand.RightRecords);
                }
            }

            return finalLand;
        }

        private async Task FillRelatedNumbersAsync(
            NpgsqlConnection connection,
            long objectId,
            string tableName,
            List<string> targetList)
        {
            string query = $@"
        SELECT {DatabaseConstants.Columns.RelatedCadastralNumber} 
        FROM {tableName} 
        WHERE {DatabaseConstants.Columns.ObjectId} = @objectId;";

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@objectId", objectId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string number = reader.GetString(0);
                if (string.IsNullOrWhiteSpace(number) == false)
                {
                    targetList.Add(number.Trim());
                }
            }
        }

        private async Task FillRightRecordsAsync(
            NpgsqlConnection connection,
            long objectId,
            List<RightRecord> targetList)
        {
            string query = $@"
        SELECT 
            {DatabaseConstants.Columns.RecordNumber}, 
            {DatabaseConstants.Columns.RightType}, 
            {DatabaseConstants.Columns.RightHolder} 
        FROM {DatabaseConstants.Tables.PropertyRights} 
        WHERE {DatabaseConstants.Columns.ObjectId} = @objectId;";

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@objectId", objectId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RightRecord record = new RightRecord(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                );
                targetList.Add(record);
            }
        }

        private async Task<CadObjectBuilt?> ReadBuiltSemanticAsync(NpgsqlConnection connection, long objectId)
        {
            string builtQuery = $@"
        SELECT 
            built.{DatabaseConstants.Columns.ExtractDate},
            built.{DatabaseConstants.Columns.CadastralNumber},
            quarter.{DatabaseConstants.Columns.QuarterNumber},
            built.{DatabaseConstants.Columns.Purpose},
            built.{DatabaseConstants.Columns.Area},
            built.{DatabaseConstants.Columns.Address},
            built.{DatabaseConstants.Columns.Cost},
            built.{DatabaseConstants.Columns.SpecialNotes},
            built.{DatabaseConstants.Columns.Extension},
            built.{DatabaseConstants.Columns.BuiltUpArea},
            built.{DatabaseConstants.Columns.Depth},
            built.{DatabaseConstants.Columns.OccurenceDepth},
            built.{DatabaseConstants.Columns.Volume},
            built.{DatabaseConstants.Columns.Height}
        FROM {DatabaseConstants.Tables.Builts} AS built
        JOIN {DatabaseConstants.Tables.Quarters} AS quarter ON built.{DatabaseConstants.Columns.QuarterId} = quarter.{DatabaseConstants.Columns.QuarterId}
        WHERE built.{DatabaseConstants.Columns.ObjectId} = @objectId;";

            CadObjectBuilt? finalBuilt = null;

            await using (NpgsqlCommand command = new NpgsqlCommand(builtQuery, connection))
            {
                command.Parameters.AddWithValue("@objectId", objectId);

                await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        finalBuilt = new CadObjectBuilt(
                            DateOnly.FromDateTime(reader.GetDateTime(0)),
                            reader.GetString(1),
                            reader.GetString(2),
                            DatabaseConstants.Discriminators.Built,
                            reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4),
                            reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            reader.IsDBNull(6) ? 0.0m : reader.GetDecimal(6),
                            reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            reader.IsDBNull(8) ? 0.0 : reader.GetDouble(8),
                            reader.IsDBNull(9) ? 0.0 : reader.GetDouble(9),
                            reader.IsDBNull(10) ? 0.0 : reader.GetDouble(10),
                            reader.IsDBNull(11) ? 0.0 : reader.GetDouble(11),
                            reader.IsDBNull(12) ? 0.0 : reader.GetDouble(12),
                            reader.IsDBNull(13) ? 0.0 : reader.GetDouble(13)
                        );
                    }
                }
            }

            if (finalBuilt != null && finalBuilt.RightRecords != null)
            {
                await FillRightRecordsAsync(connection, objectId, finalBuilt.RightRecords);
            }

            return finalBuilt;
        }

        private async Task FillBoundaryQuartersAsync(
            NpgsqlConnection connection,
            long objectId,
            List<string> targetList)
        {
            string query = $@"
        SELECT quarter.{DatabaseConstants.Columns.QuarterNumber}
        FROM {DatabaseConstants.Tables.BoundaryQuarters} AS bridge
        JOIN {DatabaseConstants.Tables.Quarters} AS quarter ON bridge.{DatabaseConstants.Columns.QuarterId} = quarter.{DatabaseConstants.Columns.QuarterId}
        WHERE bridge.{DatabaseConstants.Columns.ObjectId} = @objectId;";

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@objectId", objectId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string number = reader.GetString(0);
                if (string.IsNullOrWhiteSpace(number) == false)
                {
                    targetList.Add(number.Trim());
                }
            }
        }

        private async Task<CadObjectBoundary?> ReadBoundarySemanticAsync(NpgsqlConnection connection, long objectId)
        {
            string boundaryQuery = $@"
        SELECT 
            boundary.{DatabaseConstants.Columns.ExtractDate},
            boundary.{DatabaseConstants.Columns.RegNumbBorder},
            boundary.{DatabaseConstants.Columns.TypeBoundary},
            boundary.{DatabaseConstants.Columns.TypeZone},
            boundary.{DatabaseConstants.Columns.Description},
            boundary.{DatabaseConstants.Columns.NameByDoc}
        FROM {DatabaseConstants.Tables.Boundaries} AS boundary
        WHERE boundary.{DatabaseConstants.Columns.ObjectId} = @objectId;";

            CadObjectBoundary? finalBoundary = null;

            await using (NpgsqlCommand command = new NpgsqlCommand(boundaryQuery, connection))
            {
                command.Parameters.AddWithValue("@objectId", objectId);

                await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        finalBoundary = new CadObjectBoundary(
                            DateOnly.FromDateTime(reader.GetDateTime(0)),
                            string.Empty,
                            reader.GetString(1),
                            reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                        );
                    }
                }
            }

            if (finalBoundary != null && finalBoundary.QuarterCadNumbers != null)
            {
                await FillBoundaryQuartersAsync(connection, objectId, finalBoundary.QuarterCadNumbers);
            }

            return finalBoundary;
        }

        private async Task<CadObjectBlock?> ReadBlockSemanticAsync(NpgsqlConnection connection, long objectId)
        {
            string blockQuery = $@"
        SELECT 
            block.{DatabaseConstants.Columns.OrganRegistrRight},
            block.{DatabaseConstants.Columns.ExtractDate},
            quarter.{DatabaseConstants.Columns.QuarterNumber}
        FROM {DatabaseConstants.Tables.Blocks} AS block
        JOIN {DatabaseConstants.Tables.Quarters} AS quarter ON block.{DatabaseConstants.Columns.QuarterId} = quarter.{DatabaseConstants.Columns.QuarterId}
        WHERE block.{DatabaseConstants.Columns.ObjectId} = @objectId;";

            CadObjectBlock? finalBlock = null;

            await using (NpgsqlCommand command = new NpgsqlCommand(blockQuery, connection))
            {
                command.Parameters.AddWithValue("@objectId", objectId);

                await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        finalBlock = new CadObjectBlock(
                            reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                            DateOnly.FromDateTime(reader.GetDateTime(1)),
                            reader.GetString(2)
                        );
                    }
                }
            }

            return finalBlock;
        }
    }
}