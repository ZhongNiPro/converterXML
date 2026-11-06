using Npgsql;
using XmlConverter.Data;

namespace XmlConverter.Database
{
    public partial class CadObjectRepository
    {
        public async Task SaveObjectAsync(ICadastralObject cadObject)
        {
            await using NpgsqlConnection databaseConnection = new NpgsqlConnection(_connectionString);
            await databaseConnection.OpenAsync();
            await using NpgsqlTransaction databaseTransaction = await databaseConnection.BeginTransactionAsync();

            try
            {
                string objectTypeString = GetObjectTypeString(cadObject);
                long objectId = await InsertRegistryAsync(databaseConnection, databaseTransaction, objectTypeString);

                switch (cadObject)
                {
                    case CadObjectLand land:
                        await LandHandler(databaseConnection, databaseTransaction, land, objectId);
                        break;

                    case CadObjectBuilt built:
                        await BuiltHandler(databaseConnection, databaseTransaction, built, objectId);
                        break;

                    case CadObjectBoundary boundary:
                        await BoundaryHandler(databaseConnection, databaseTransaction, boundary, objectId);
                        break;

                    case CadObjectBlock block:
                        await BlockHandler(databaseConnection, databaseTransaction, block, objectId);
                        break;
                }

                await databaseTransaction.CommitAsync();
            }
            catch (Exception)
            {
                await databaseTransaction.RollbackAsync();

                throw;
            }
        }

        private async Task LandHandler(NpgsqlConnection databaseConnection, NpgsqlTransaction databaseTransaction, CadObjectLand land, long objectId)
        {
            long quarterId = await GetOrCreateQuarterAsync(databaseConnection, databaseTransaction, land.QuarterCadNumber);

            await InsertLandSpecificationAsync(databaseConnection, databaseTransaction, objectId, land, quarterId);

            if (land.AscendantCadNumbers != null && land.AscendantCadNumbers.Count > 0)
            {
                await InsertLandRelatedNumbersAsync(databaseConnection, databaseTransaction, objectId, land.AscendantCadNumbers, DatabaseConstants.Tables.LandAscendants);
            }

            if (land.DescendantCadNumbers != null && land.DescendantCadNumbers.Count > 0)
            {
                await InsertLandRelatedNumbersAsync(databaseConnection, databaseTransaction, objectId, land.DescendantCadNumbers, DatabaseConstants.Tables.LandDescendants);
            }

            if (land.IncludedCadNumbers != null && land.IncludedCadNumbers.Count > 0)
            {
                await InsertLandRelatedNumbersAsync(databaseConnection, databaseTransaction, objectId, land.IncludedCadNumbers, DatabaseConstants.Tables.LandIncluded);
            }

            if (land.ObjectParts != null && land.ObjectParts.Count > 0)
            {
                await InsertLandPartsAsync(databaseConnection, databaseTransaction, objectId, land.ObjectParts);
            }

            if (land.RightRecords != null && land.RightRecords.Count > 0)
            {
                await InsertRightRecordsAsync(databaseConnection, databaseTransaction, objectId, land.RightRecords);
            }

            await InsertGeometryAsync(databaseConnection, databaseTransaction, objectId, land.Contours);
        }

        private async Task BuiltHandler(NpgsqlConnection databaseConnection, NpgsqlTransaction databaseTransaction, CadObjectBuilt built, long objectId)
        {
            long builtQuarterId = await GetOrCreateQuarterAsync(databaseConnection, databaseTransaction, built.QuarterCadNumber);

            await InsertBuiltSpecificationAsync(databaseConnection, databaseTransaction, objectId, built, builtQuarterId);

            if (built.RightRecords != null && built.RightRecords.Count > 0)
            {
                await InsertRightRecordsAsync(databaseConnection, databaseTransaction, objectId, built.RightRecords);
            }

            await InsertGeometryAsync(databaseConnection, databaseTransaction, objectId, built.Contours);
        }

        private async Task BoundaryHandler(NpgsqlConnection databaseConnection, NpgsqlTransaction databaseTransaction, CadObjectBoundary boundary, long objectId)
        {
            await InsertBoundarySpecificationAsync(databaseConnection, databaseTransaction, objectId, boundary);

            if (boundary.QuarterCadNumbers != null && boundary.QuarterCadNumbers.Count > 0)
            {
                await LinkBoundaryToQuartersAsync(databaseConnection, databaseTransaction, objectId, boundary.QuarterCadNumbers);
            }

            await InsertGeometryAsync(databaseConnection, databaseTransaction, objectId, boundary.Contours);
        }

        private async Task BlockHandler(NpgsqlConnection databaseConnection, NpgsqlTransaction databaseTransaction, CadObjectBlock block, long objectId)
        {
            long blockQuarterId = await GetOrCreateQuarterAsync(databaseConnection, databaseTransaction, block.CadastralNumber);

            await InsertBlockSpecificationAsync(databaseConnection, databaseTransaction, objectId, block, blockQuarterId);

            if (block.Contours != null && block.Contours.Count > 0)
            {
                await InsertGeometryAsync(databaseConnection, databaseTransaction, objectId, block.Contours);
            }
        }

        private string GetObjectTypeString(ICadastralObject cadObject)
        {
            return cadObject switch
            {
                CadObjectLand => DatabaseConstants.Discriminators.Land,
                CadObjectBuilt => DatabaseConstants.Discriminators.Built,
                CadObjectBoundary => DatabaseConstants.Discriminators.Boundary,
                CadObjectBlock => DatabaseConstants.Discriminators.Block,
                _ => throw new ArgumentException($"{DatabaseConstants.Discriminators.UnknownType}: {cadObject.GetType()}")
            };
        }

        private async Task<long> InsertRegistryAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            string objectType)
        {
            string insertSql = $"INSERT INTO {DatabaseConstants.Tables.Registry} ({DatabaseConstants.Columns.ObjectType}) VALUES (@objectType) RETURNING {DatabaseConstants.Columns.ObjectId};";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);
            sqlCommand.Parameters.AddWithValue("@objectType", objectType);

            object? result = await sqlCommand.ExecuteScalarAsync();

            return Convert.ToInt64(result);
        }

        private async Task InsertLandSpecificationAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            CadObjectLand land,
            long quarterId)
        {
            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Lands} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.CadastralNumber}, 
                {DatabaseConstants.Columns.ExtractDate}, 
                {DatabaseConstants.Columns.SubType}, 
                {DatabaseConstants.Columns.Category}, 
                {DatabaseConstants.Columns.PermittedUse}, 
                {DatabaseConstants.Columns.Area}, 
                {DatabaseConstants.Columns.Inaccuracy}, 
                {DatabaseConstants.Columns.Address}, 
                {DatabaseConstants.Columns.Cost}, 
                {DatabaseConstants.Columns.SpecialNotes}, 
                {DatabaseConstants.Columns.QuarterId}
            ) 
        VALUES 
            (
                @objectId, 
                @cadastralNumber, 
                @extractDate, 
                @subType, 
                @category, 
                @permittedUse, 
                @area, 
                @inaccuracy, 
                @address, 
                @cost, 
                @specialNotes, 
                @quarterId
            )
        ON CONFLICT ({DatabaseConstants.Columns.CadastralNumber}, {DatabaseConstants.Columns.ExtractDate}) DO NOTHING;";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.AddWithValue("@objectId", objectId);
            sqlCommand.Parameters.AddWithValue("@cadastralNumber", land.CadNumber);
            sqlCommand.Parameters.AddWithValue("@extractDate", land.DateFormation);
            sqlCommand.Parameters.AddWithValue("@subType", (object)land.SubType ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@category", (object)land.Category ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@permittedUse", (object)land.PermittedUse ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@area", land.Area);
            sqlCommand.Parameters.AddWithValue("@inaccuracy", (object)land.Inaccuracy ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@address", (object)land.Address ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@cost", (object)land.Cost ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@specialNotes", (object)land.SpesialNotes ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@quarterId", quarterId);

            await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task InsertLandRelatedNumbersAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            List<string> cadastralNumbers,
            string targetTableName)
        {
            string insertSql = $@"
        INSERT INTO {targetTableName} 
            ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.RelatedCadastralNumber}) 
        VALUES 
            (@objectId, @relatedNumber);";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.Add("@objectId", NpgsqlTypes.NpgsqlDbType.Bigint);
            sqlCommand.Parameters.Add("@relatedNumber", NpgsqlTypes.NpgsqlDbType.Text);

            await sqlCommand.PrepareAsync();

            sqlCommand.Parameters["@objectId"].Value = objectId;

            foreach (string number in cadastralNumbers)
            {
                if (string.IsNullOrWhiteSpace(number))
                    continue;

                sqlCommand.Parameters["@relatedNumber"].Value = number.Trim();
                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertLandPartsAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long parentObjectId,
            List<ObjectPart> objectParts)
        {
            string insertPartSql = $@"
        INSERT INTO {DatabaseConstants.Tables.LandParts} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.ParentObjectId}, 
                {DatabaseConstants.Columns.PartNumber}
            ) 
        VALUES 
            (@partObjectId, @parentObjectId, @partNumber);";

            foreach (ObjectPart part in objectParts)
            {
                if (part == null)
                    continue;

                long partObjectId = await InsertRegistryAsync(databaseConnection, databaseTransaction, DatabaseConstants.Discriminators.LandPart);

                await using (NpgsqlCommand sqlCommand = new NpgsqlCommand(insertPartSql, databaseConnection, databaseTransaction))
                {
                    sqlCommand.Parameters.AddWithValue("@partObjectId", partObjectId);
                    sqlCommand.Parameters.AddWithValue("@parentObjectId", parentObjectId);
                    sqlCommand.Parameters.AddWithValue("@partNumber", part.PartNumber);

                    await sqlCommand.ExecuteNonQueryAsync();
                }

                if (part.PartsContours != null && part.PartsContours.Count > 0)
                {
                    await InsertGeometryAsync(databaseConnection, databaseTransaction, partObjectId, part.PartsContours);
                }
            }
        }

        private async Task InsertRightRecordsAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            List<RightRecord> rightRecords)
        {
            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.PropertyRights} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.RecordNumber}, 
                {DatabaseConstants.Columns.RightType}, 
                {DatabaseConstants.Columns.RightHolder}
            ) 
        VALUES 
            (@objectId, @recordNumber, @rightType, @rightHolder);";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.Add("@objectId", NpgsqlTypes.NpgsqlDbType.Bigint);
            sqlCommand.Parameters.Add("@recordNumber", NpgsqlTypes.NpgsqlDbType.Integer);
            sqlCommand.Parameters.Add("@rightType", NpgsqlTypes.NpgsqlDbType.Text);
            sqlCommand.Parameters.Add("@rightHolder", NpgsqlTypes.NpgsqlDbType.Text);

            await sqlCommand.PrepareAsync();

            sqlCommand.Parameters["@objectId"].Value = objectId;

            foreach (RightRecord record in rightRecords)
            {
                if (record == null)
                    continue;

                sqlCommand.Parameters["@recordNumber"].Value = record.RecordNumber;
                sqlCommand.Parameters["@rightType"].Value = record.RightType ?? string.Empty;
                sqlCommand.Parameters["@rightHolder"].Value = record.RightHolder ?? string.Empty;

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertBuiltSpecificationAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            CadObjectBuilt built,
            long quarterId)
        {
            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Builts} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.CadastralNumber}, 
                {DatabaseConstants.Columns.ExtractDate}, 
                {DatabaseConstants.Columns.Purpose}, 
                {DatabaseConstants.Columns.Area}, 
                {DatabaseConstants.Columns.Address}, 
                {DatabaseConstants.Columns.Cost}, 
                {DatabaseConstants.Columns.SpecialNotes}, 
                {DatabaseConstants.Columns.Extension}, 
                {DatabaseConstants.Columns.BuiltUpArea}, 
                {DatabaseConstants.Columns.Depth}, 
                {DatabaseConstants.Columns.OccurenceDepth}, 
                {DatabaseConstants.Columns.Volume}, 
                {DatabaseConstants.Columns.Height}, 
                {DatabaseConstants.Columns.QuarterId}
            ) 
        VALUES 
            (
                @objectId, 
                @cadastralNumber, 
                @extractDate, 
                @purpose, 
                @area, 
                @address, 
                @cost, 
                @specialNotes, 
                @extension, 
                @builtUpArea, 
                @depth, 
                @occurenceDepth, 
                @volume, 
                @height, 
                @quarterId
            )
        ON CONFLICT ({DatabaseConstants.Columns.CadastralNumber}, {DatabaseConstants.Columns.ExtractDate}) DO NOTHING;";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.AddWithValue("@objectId", objectId);
            sqlCommand.Parameters.AddWithValue("@cadastralNumber", built.CadNumber);
            sqlCommand.Parameters.AddWithValue("@extractDate", built.DateFormation);
            sqlCommand.Parameters.AddWithValue("@purpose", (object)built.Purpose ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@area", (object)built.Area ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@address", (object)built.Address ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@cost", (object)built.Cost ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@specialNotes", (object)built.SpesialNotes ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@extension", (object)built.Extension ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@builtUpArea", (object)built.BuiltUpArea ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@depth", (object)built.Depth ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@occurenceDepth", (object)built.OccurenceDepth ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@volume", (object)built.Volume ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@height", (object)built.Height ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@quarterId", quarterId);

            await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task InsertBoundarySpecificationAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            CadObjectBoundary boundary)
        {
            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Boundaries} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.RegNumbBorder}, 
                {DatabaseConstants.Columns.ExtractDate}, 
                {DatabaseConstants.Columns.TypeBoundary}, 
                {DatabaseConstants.Columns.TypeZone}, 
                {DatabaseConstants.Columns.Description}, 
                {DatabaseConstants.Columns.NameByDoc}
            ) 
        VALUES 
            (
                @objectId, 
                @regNumbBorder, 
                @extractDate, 
                @typeBoundary, 
                @typeZone, 
                @description, 
                @nameByDoc
            )
        ON CONFLICT ({DatabaseConstants.Columns.RegNumbBorder}, {DatabaseConstants.Columns.ExtractDate}) DO NOTHING;";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.AddWithValue("@objectId", objectId);
            sqlCommand.Parameters.AddWithValue("@regNumbBorder", boundary.RegNumbBorder);
            sqlCommand.Parameters.AddWithValue("@extractDate", boundary.DateFormation);
            sqlCommand.Parameters.AddWithValue("@typeBoundary", (object)boundary.TypeBoundary ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@typeZone", (object)boundary.TypeZone ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@description", (object)boundary.Description ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@nameByDoc", (object)boundary.NameByDoc ?? DBNull.Value);

            await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task LinkBoundaryToQuartersAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            List<string> quarterNumbers)
        {
            string insertLinkSql = $@"
        INSERT INTO {DatabaseConstants.Tables.BoundaryQuarters} 
            ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.QuarterId}) 
        VALUES 
            (@objectId, @quarterId)
        ON CONFLICT DO NOTHING;"; 

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertLinkSql, databaseConnection, databaseTransaction);
            sqlCommand.Parameters.Add("@objectId", NpgsqlTypes.NpgsqlDbType.Bigint);
            sqlCommand.Parameters.Add("@quarterId", NpgsqlTypes.NpgsqlDbType.Bigint);

            await sqlCommand.PrepareAsync();

            sqlCommand.Parameters["@objectId"].Value = objectId;

            foreach (string quarterNum in quarterNumbers)
            {
                if (string.IsNullOrWhiteSpace(quarterNum)) continue;

                long quarterId = await GetOrCreateQuarterAsync(databaseConnection, databaseTransaction, quarterNum);

                sqlCommand.Parameters["@quarterId"].Value = quarterId;
                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertBlockSpecificationAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            CadObjectBlock block,
            long quarterId)
        {
            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Blocks} 
            (
                {DatabaseConstants.Columns.ObjectId}, 
                {DatabaseConstants.Columns.QuarterId}, 
                {DatabaseConstants.Columns.OrganRegistrRight}, 
                {DatabaseConstants.Columns.ExtractDate}
            ) 
        VALUES 
            (
                @objectId, 
                @quarterId, 
                @organRegistrRight, 
                @extractDate
            )
        ON CONFLICT ({DatabaseConstants.Columns.QuarterId}, {DatabaseConstants.Columns.ExtractDate}) DO NOTHING;";

            await using NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction);

            sqlCommand.Parameters.AddWithValue("@objectId", objectId);
            sqlCommand.Parameters.AddWithValue("@quarterId", quarterId);
            sqlCommand.Parameters.AddWithValue("@organRegistrRight", (object)block.OrganRegistrRight ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@extractDate", block.DateFormation);

            await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task<long> GetOrCreateQuarterAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            string quarterNumber)
        {
            string cleanNumber = string.IsNullOrWhiteSpace(quarterNumber) ? string.Empty : quarterNumber.Trim();

            string selectSql = $@"
        SELECT {DatabaseConstants.Columns.QuarterId} 
        FROM {DatabaseConstants.Tables.Quarters} 
        WHERE {DatabaseConstants.Columns.QuarterNumber} = @quarter;";

            await using (NpgsqlCommand sqlCommand = new NpgsqlCommand(selectSql, databaseConnection, databaseTransaction))
            {
                sqlCommand.Parameters.AddWithValue("@quarter", cleanNumber);
                object? result = await sqlCommand.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt64(result);
                }
            }

            string insertSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Quarters} 
            ({DatabaseConstants.Columns.QuarterNumber}) 
        VALUES 
            (@quarter) 
        RETURNING {DatabaseConstants.Columns.QuarterId};";

            await using (NpgsqlCommand sqlCommand = new NpgsqlCommand(insertSql, databaseConnection, databaseTransaction))
            {
                sqlCommand.Parameters.AddWithValue("@quarter", cleanNumber);
                object? newId = await sqlCommand.ExecuteScalarAsync();

                return Convert.ToInt64(newId);
            }
        }

        private async Task InsertGeometryAsync(
            NpgsqlConnection databaseConnection,
            NpgsqlTransaction databaseTransaction,
            long objectId,
            List<Contour>? contours)
        {
            if (contours == null || contours.Count == 0)
                return;

            string insertContourSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Contours} 
            ({DatabaseConstants.Columns.ObjectId}, {DatabaseConstants.Columns.ContourNumber}) 
        VALUES 
            (@objectId, @contourNumber)
        RETURNING {DatabaseConstants.Columns.ContourId};";

            string insertCoordSql = $@"
        INSERT INTO {DatabaseConstants.Tables.Coordinates} 
            ({DatabaseConstants.Columns.ContourId}, {DatabaseConstants.Columns.OrdNmb}, {DatabaseConstants.Columns.X}, {DatabaseConstants.Columns.Y}) 
        VALUES 
            (@contourId, @ordNmb, @x, @y);";

            await using NpgsqlCommand coordCommand = new NpgsqlCommand(insertCoordSql, databaseConnection, databaseTransaction);

            coordCommand.Parameters.Add("@contourId", NpgsqlTypes.NpgsqlDbType.Bigint);
            coordCommand.Parameters.Add("@ordNmb", NpgsqlTypes.NpgsqlDbType.Integer);
            coordCommand.Parameters.Add("@x", NpgsqlTypes.NpgsqlDbType.Double);
            coordCommand.Parameters.Add("@y", NpgsqlTypes.NpgsqlDbType.Double);

            await coordCommand.PrepareAsync();

            foreach (Contour contour in contours)
            {
                if (contour == null)
                    continue;

                long contourId;

                await using (NpgsqlCommand contourCommand = new NpgsqlCommand(insertContourSql, databaseConnection, databaseTransaction))
                {
                    contourCommand.Parameters.AddWithValue("@objectId", objectId);
                    contourCommand.Parameters.AddWithValue("@contourNumber", contour.NumberPP);

                    object? result = await contourCommand.ExecuteScalarAsync();
                    contourId = Convert.ToInt64(result);
                }

                if (contour.Coordinates != null && contour.Coordinates.Count > 0)
                {
                    foreach (Coordinate coordinate in contour.Coordinates)
                    {
                        if (coordinate == null)
                            continue;

                        coordCommand.Parameters["@contourId"].Value = contourId;
                        coordCommand.Parameters["@ordNmb"].Value = coordinate.OrdNmb;
                        coordCommand.Parameters["@x"].Value = coordinate.X;
                        coordCommand.Parameters["@y"].Value = coordinate.Y;

                        await coordCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}
