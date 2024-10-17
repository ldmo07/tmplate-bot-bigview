using Azure.Storage.Blobs.Models;

namespace Bigview.Bot.Core.Components.Storage;

public interface IStorageServices
{
    IEnumerable<BlobItem> GetFiles(string containerName);
    Task<string> UploadFile(string containerName, string fileName, Stream uploadFileStream, IDictionary<string, string> metadata = null, bool overrideFile = true);
    Task<string> UploadFile(string containerName, string fileName, Stream uploadFileStream, string contentType, IDictionary<string, string> metadata = null, bool overrideFile = true);
}