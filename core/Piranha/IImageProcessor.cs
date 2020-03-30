/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.IO;

namespace Piranha
{
    /// <summary>
    /// Interface for an image processor.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Gets an image from the provided stream and returns its size.
        /// </summary>
        /// <param name="stream">The image data stream</param>
        /// <param name="width">The returned width</param>
        /// <param name="height">The returned height</param>
        void GetSize(Stream stream, out int width, out int height);

        /// <summary>
        /// Gets an image from the provided bytes and returns its size.
        /// </summary>
        /// <param name="bytes">The image data</param>
        /// <param name="width">The returned width</param>
        /// <param name="height">The returned height</param>
        void GetSize(byte[] bytes, out int width, out int height);

        /// <summary>
        /// Gets an image from the provided stream, crops it according
        /// to the given size and writes out a new jpeg image on the
        /// destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The requested height</param>
        void Crop(Stream source, Stream dest, int width, int height);

        /// <summary>
        /// Gets an image from the provided stream, scales it according
        /// to the given width and writes out a new jpeg image on the
        /// destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        void Scale(Stream source, Stream dest, int width);

        /// <summary>
        /// Gets an image from the provided stream, crops and scales it
        /// according to the given size and writes out a new jpeg image
        /// on the destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The requested height</param>
        void CropScale(Stream source, Stream dest, int width, int height);
    }
}