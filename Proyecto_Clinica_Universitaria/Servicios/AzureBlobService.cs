using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Proyecto_Clinica_Universitaria.Servicios
{
    public class AzureBlobService
    {
        private readonly BlobContainerClient _container;

        public AzureBlobService(IConfiguration cfg)
        {
            var cs = cfg["AzureBlob:ConnectionString"];
            var containerName = cfg["AzureBlob:ContainerName"];

            _container = new BlobContainerClient(cs, containerName);
            // Si el contenedor ya existe, no pasa nada; si no, lo crea.
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<(string blobName, string url)> UploadAsync(
            Stream content, string originalFileName, string contentType, CancellationToken ct = default)
        {
            // Nombre único y “carpetas” por fecha
            var safeName = string.Concat(originalFileName.Split(Path.GetInvalidFileNameChars()));
            var blobName = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}_{safeName}".ToLowerInvariant();

            var blob = _container.GetBlobClient(blobName);

            await blob.UploadAsync(content, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            }, ct);

            return (blobName, blob.Uri.ToString()); // URL pública directa
        }

        public async Task DeleteIfExistsAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)) return;
            var blob = _container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}


