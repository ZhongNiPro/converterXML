using System.Diagnostics;
using System.Xml;
using XmlConverter.Data;
using XmlConverter.Database;
using static XmlConverter.Business.CadFileProvider;

namespace XmlConverter.Business
{
    public class XmlStreamDispatcher
    {
        private readonly CadFileProvider _fileProvider;
        private readonly CadObjectRepository _repository;

        public XmlStreamDispatcher(string connectionString)
        {
            _fileProvider = new CadFileProvider();
            _repository = new CadObjectRepository(connectionString);
        }

        public void UpdateConnectionString(string newConnectionString)
        {
            if (string.IsNullOrWhiteSpace(newConnectionString))
            {
                throw new ArgumentException("Строка подключения не может быть пустой.");
            }

            _repository.UpdateConnectionString(newConnectionString);
        }

        public async Task RunParsingPipelineAsync(
            List<string> filePaths,
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            if (filePaths == null || filePaths.Count == 0)
                return;

            await foreach (CadFileStreamData fileData in _fileProvider.GetXmlStreamsAsync(filePaths, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await ParseSingleXmlStreamAsync(fileData.FileStream, progress, cancellationToken);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка парсинга XML-потока {fileData.FileName}: {ex.Message}");
                }
                finally
                {
                    await fileData.FileStream.DisposeAsync();
                }
            }
        }

        private async Task ParseSingleXmlStreamAsync(Stream xmlStream, IProgress<double> progress, CancellationToken cancellationToken)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true,
                IgnoreWhitespace = true
            };

            using (XmlReader reader = XmlReader.Create(xmlStream, settings))
            {
                DateOnly dateFormation = DateOnly.FromDateTime(DateTime.Now);
                string quarterCadNumber = string.Empty;
                long totalBytes = xmlStream.Length;

                bool isDateCaptured = false;
                bool isQuarterCaptured = false;

                ParserFactory factory = new ParserFactory();

                while (await reader.ReadAsync())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (isDateCaptured == false && reader.Name == CadastralConstants.XmlContentTags.DateFormationTag)
                        {
                            string rawDate = await reader.ReadInnerXmlAsync();
                            DateOnly.TryParse(rawDate, System.Globalization.CultureInfo.InvariantCulture, out dateFormation);
                            isDateCaptured = true;
                            continue;
                        }

                        if (isQuarterCaptured == false && reader.Name == CadastralConstants.XmlContentTags.CadastralNumberTag)
                        {
                            quarterCadNumber = await reader.ReadInnerXmlAsync();
                            isQuarterCaptured = true;
                            continue;
                        }

                        dynamic? parser = factory.GetParser(reader.Name, dateFormation, quarterCadNumber);

                        if (parser != null)
                        {
                            ICadastralObject cadastralObject = await parser.ParseAsync(reader);

                            await _repository.SaveObjectAsync(cadastralObject);

                            double percentage = ((double)xmlStream.Position / totalBytes) * 100;
                            progress.Report(percentage);
                        }
                    }
                }
            }
        }
    }
}