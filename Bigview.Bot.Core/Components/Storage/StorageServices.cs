﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Bigview.Bot.Core.Components.Storage.Models;

namespace Bigview.Bot.Core.Components.Storage;

public class StorageServices : IStorageServices
{
    /// <summary>
    /// The BLOB service client
    /// </summary>
    /// <autogeneratedoc />

    readonly BlobServiceClient blobServiceClient;
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageServices"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <autogeneratedoc />

    public StorageServices(StorageAccountParameters storageAccount)
    {
        blobServiceClient = new BlobServiceClient(storageAccount.ConnectionString);
    }

    /// <summary>
    /// Uploads the file.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="uploadFileStream">The upload file stream.</param>
    /// <returns></returns>
    /// <autogeneratedoc />

    public async Task<string> UploadFile(string containerName, string fileName, Stream uploadFileStream, IDictionary<string, string> metadata = null, bool overrideFile = true)
    {

        // Create the container and return a container client object
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Get a reference to a blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Open the file and upload its data
        await blobClient.UploadAsync(uploadFileStream, overwrite: overrideFile);

        //validate if exist metadata 
        if (metadata != null && metadata.Any())
            await blobClient.SetMetadataAsync(metadata);

        uploadFileStream.Close();

        return blobClient.Uri.ToString();
    }

    public async Task<string> UploadFile(string containerName, string fileName, Stream uploadFileStream, string contentType, IDictionary<string, string> metadata = null, bool overrideFile = true)
    {

        // Create the container and return a container client object
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Get a reference to a blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Open the file and upload its data
        await blobClient.UploadAsync(uploadFileStream, overwrite: overrideFile);

        //set content type
        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });
        //validate if exist metadata 
        if (metadata != null && metadata.Any())
            await blobClient.SetMetadataAsync(metadata);

        uploadFileStream.Close();

        var uriSAS = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));

        return uriSAS.ToString();
    }



    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="uploadFileStream">The upload file stream.</param>
    /// <returns></returns>
    /// <autogeneratedoc />
    public IEnumerable<BlobItem> GetFiles(string containerName)
    {
        // Create the container and return a container client object
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        return containerClient.GetBlobs(Azure.Storage.Blobs.Models.BlobTraits.All);
    }
}