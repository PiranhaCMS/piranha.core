/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Local;

public class FileStorageSession : IStorageSession
{
    private readonly FileStorage _storage;
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly FileStorageNaming _naming;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="storage">The main file storage</param>
    /// <param name="basePath">The base path</param>
    /// <param name="baseUrl">The base url</param>
    /// <param name="naming">How uploaded media files should be named</param>
    public FileStorageSession(FileStorage storage, string basePath, string baseUrl, FileStorageNaming naming)
    {
        _storage = storage;
        _basePath = basePath;
        _baseUrl = baseUrl;
        _naming = naming;
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
        var path = _storage.GetResourceName(media, filename);

        if (File.Exists(_basePath + path))
        {
            using (var file = File.OpenRead(_basePath + path))
            {
                await file.CopyToAsync(stream).ConfigureAwait(false);
                return true;
            }
        }
        return false;
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
        var path = _storage.GetResourceName(media, filename);

        EnsureFolder(media);

        using (var file = File.OpenWrite(_basePath + path))
        {
            await stream.CopyToAsync(file).ConfigureAwait(false);
        }
        return _baseUrl + path;
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
        var path = _storage.GetResourceName(media, filename);

        EnsureFolder(media);

        using (var file = File.OpenWrite(_basePath + path))
        {
            await file.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }
        return _baseUrl + path;
    }

    /// <summary>
    /// Deletes the content for the specified media.
    /// </summary>
    /// <param name="media">The media file</param>
    /// <param name="filename">The file name</param>
    public Task<bool> DeleteAsync(Media media, string filename)
    {
        return Task.Run(() =>
        {
            var path = _storage.GetResourceName(media, filename);

            if (File.Exists(_basePath + path))
            {
                File.Delete(_basePath + path);

                if (_naming == FileStorageNaming.UniqueFolderNames)
                {
                    var folderPath = $"{ _basePath }/{ media.Id }";

                    // Check if the folder is empty, and if so, delete it
                    if (Directory.GetFiles(folderPath).Length == 0)
                    {
                        Directory.Delete(folderPath);
                    }
                }

                return true;
            }
            return false;
        });
    }

    /// <summary>
    /// Disposes the session.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Makes sure the folder exists if unique folder names
    /// are used.
    /// </summary>
    /// <param name="media">The media file</param>
    private void EnsureFolder(Media media)
    {
        if (_naming == FileStorageNaming.UniqueFolderNames)
        {
            if (!Directory.Exists($"{ _basePath }/{ media.Id }"))
            {
                Directory.CreateDirectory($"{ _basePath }/{ media.Id }");
            }
        }
    }
}
