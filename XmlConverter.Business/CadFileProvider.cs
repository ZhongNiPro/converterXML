using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace XmlConverter.Business
{
    public class CadFileProvider
    {
        public record CadFileStreamData(Stream FileStream, string FileName);

        private const string XmlExtension = ".xml";
        private const string ZipExtension = ".zip";

        public async IAsyncEnumerable<CadFileStreamData> GetXmlStreamsAsync(
            List<string> filePaths,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (string path in filePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(path) == false)
                    continue;

                string extension = Path.GetExtension(path).ToLowerInvariant();

                if (extension == XmlExtension)
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                    string fileName = Path.GetFileName(path);

                    yield return new CadFileStreamData(fileStream, fileName);
                }
                else if (extension == ZipExtension)
                {
                    await foreach (CadFileStreamData zipEntryData in GetStreamsFromZipAsync(path, cancellationToken))
                    {
                        yield return zipEntryData;
                    }
                }
            }
        }

        private async IAsyncEnumerable<CadFileStreamData> GetStreamsFromZipAsync(
            string zipPath,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            FileStream zipFileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            ZipArchive archive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (entry.FullName.EndsWith(XmlExtension, StringComparison.OrdinalIgnoreCase))
                {
                    Stream entryStream = entry.Open();

                    yield return new CadFileStreamData(entryStream, entry.Name);
                }
            }
        }
    }
}