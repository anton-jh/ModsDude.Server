using Azure.Storage.Blobs;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Mods;

namespace ModsDude.Server.Storage.Services;
internal class StorageService(
    BlobServiceClient blobServiceClient)
    : IStorageService
{
    private const string _modsContainerName = "mods";


    public async Task<bool> CheckIfModExists(ModId modId, ModVersionId versionId, CancellationToken cancellationToken)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(_modsContainerName);
        var blobClient = containerClient.GetBlobClient(BuildModFilename(modId, versionId));

        var result = await blobClient.ExistsAsync(cancellationToken);
        return result.Value;
    }


    private static string BuildModFilename(ModId modId, ModVersionId versionId)
    {
        return $"{modId.Value}/{versionId.Value}";
    }
}
