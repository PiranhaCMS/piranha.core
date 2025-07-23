/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Piranha.Models;

namespace Piranha.Azure;

public class BlobStorage : IStorage, IStorageSession
{
    /// <summary>
    /// The private storage account.
    /// </summary>
    private readonly BlobContainerClient _blobContainerClient;

    /// <summary>
    /// How uploaded files should be named to
    /// ensure uniqueness.
    /// </summary>
    private readonly BlobStorageNaming _naming;

    /// <summary>
    /// Creates a new Blog Storage service from the given credentials.
    /// </summary>
    /// <param name="blobContainerUri">
    /// A <see cref="Uri"/> referencing the blob service.
    /// This is likely to be similar to "https://{account_name}.blob.core.windows.net".
    /// </param>
    /// <param name="tokenCredential">The connection credentials</param>
    /// <param name="naming">How uploaded media files should be named</param>
    public BlobStorage(
        Uri blobContainerUri,
        TokenCredential tokenCredential,
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames)
    {
        _blobContainerClient = new BlobContainerClient(blobContainerUri, tokenCredential);
        _naming = naming;

        // Ensure that blob container exists
        _blobContainerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    /// <summary>
    /// Creates a new Blob Storage service from the given connection string.
    /// </summary>
    /// <param name="connectionString">The connection string</param>
    /// <param name="containerName">The container name</param>
    /// <param name="naming">How uploaded media files should be named</param>
    public BlobStorage(
        string connectionString,
        string containerName = "uploads",
        BlobStorageNaming naming = BlobStorageNaming.UniqueFileNames)
    {
        _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        _naming = naming;
    }

    public Task<IStorageSession> OpenAsync()
    {
        return Task.FromResult<IStorageSession>(this);
    }

    /// <summary>
    /// Gets the public URL for the given media object.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="id">The resource id</param>
    /// <returns>The public url</returns>
    public string GetPublicUrl(Media media, string id)
    {
        return media == null || string.IsNullOrWhiteSpace(id) ? null :
            $"{ _blobContainerClient.Uri.AbsoluteUri }/{ GetResourceName(media, id, true) }";
    }

    /// <summary>
    /// Gets the resource name for the given media object.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <returns>The public url</returns>
    public string GetResourceName(Media media, string filename)
    {
        return GetResourceName(media, filename, false);
    }

    /// <summary>
    /// Gets the resource name for the given media object.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <param name="encode">If the filename should be URL encoded</param>
    /// <returns>The public url</returns>
    public string GetResourceName(Media media, string filename, bool encode)
    {
        return _naming == BlobStorageNaming.UniqueFileNames ?
            $"{ media.Id }-{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }" :
            $"{ media.Id }/{ (encode ? System.Web.HttpUtility.UrlPathEncode(filename) : filename) }";
    }

    /// <summary>
    /// Writes the content for the specified media content to the given stream.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <param name="stream">The output stream</param>
    /// <returns>If the media was found</returns>
    public async Task<bool> GetAsync(Media media, string filename, Stream stream)
    {
        var blob = _blobContainerClient.GetBlobClient(GetResourceName(media, filename));

        return await blob.ExistsAsync() && (await blob.DownloadToAsync(stream)).Status.IsSuccessStatusCode();
    }

    /// <summary>
    /// Stores the given media content.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <param name="contentType">The content type</param>
    /// <param name="stream">The input stream</param>
    /// <returns>The public URL</returns>
    public async Task<string> PutAsync(Media media, string filename, string contentType, Stream stream)
    {
        var blob = _blobContainerClient.GetBlobClient(GetResourceName(media, filename));

        var blobHttpHeader = new BlobHttpHeaders {ContentType = contentType};

        await blob.UploadAsync(stream, blobHttpHeader);

        return blob.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Stores the given media content.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    /// <param name="contentType">The content type</param>
    /// <param name="bytes">The binary data</param>
    /// <returns>The public URL</returns>
    public async Task<string> PutAsync(Media media, string filename, string contentType, byte[] bytes)
    {
        return await PutAsync(media, filename, contentType, new MemoryStream(bytes));
    }

    /// <summary>
    /// Deletes the content for the specified media.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    public async Task<bool> DeleteAsync(Media media, string filename)
    {
        var blob = _blobContainerClient.GetBlobClient(GetResourceName(media, filename));

        return await blob.DeleteIfExistsAsync();
    }

    /// <summary>
    /// Disposes the session.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
