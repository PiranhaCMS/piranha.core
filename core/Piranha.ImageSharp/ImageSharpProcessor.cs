/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace Piranha.ImageSharp
{
    public class ImageSharpProcessor : IImageProcessor
    {
        /// <summary>
        /// Gets an image from the provided stream and returns its size.
        /// </summary>
        /// <param name="stream">The image data stream</param>
        /// <param name="width">The returned width</param>
        /// <param name="height">The returned height</param>
        public void GetSize(Stream stream, out int width, out int height)
        {
            using (var image = Image.Load(stream))
            {
                width = image.Width;
                height = image.Height;
            }
        }

        /// <summary>
        /// Gets an image from the provided bytes and returns its size.
        /// </summary>
        /// <param name="bytes">The image data</param>
        /// <param name="width">The returned width</param>
        /// <param name="height">The returned height</param>
        public void GetSize(byte[] bytes, out int width, out int height)
        {
            using (var image = Image.Load(bytes))
            {
                width = image.Width;
                height = image.Height;
            }
        }

        /// <summary>
        /// Gets an image from the provided stream, crops it according
        /// to the given size and writes out a new jpeg image on the
        /// destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The requested height</param>
        public void Crop(Stream source, Stream dest, int width, int height)
        {
            using (var image = Image.Load(source, out IImageFormat format))
            {
                image.Mutate(x => x.Crop(new Rectangle
                {
                    Width = width,
                    Height = height,
                    X = width < image.Width ? (image.Width - width) / 2 : 0,
                    Y = height < image.Height ? (image.Height - height) / 2 : 0
                }));

                image.Save(dest, format);
            }
        }

        /// <summary>
        /// Gets an image from the provided stream, scales it according
        /// to the given width and writes out a new jpeg image on the
        /// destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        public void Scale(Stream source, Stream dest, int width)
        {
            using (var image = Image.Load(source, out IImageFormat format))
            {
                int height = (int)Math.Round(width * ((float)image.Height / image.Width));

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                image.Save(dest, format);
            }
        }

        /// <summary>
        /// Gets an image from the provided stream, crops and scales it
        /// according to the given size and writes out a new jpeg image
        /// on the destination stream.
        /// </summary>
        /// <param name="source">The image data stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The requested height</param>
        public void CropScale(Stream source, Stream dest, int width, int height)
        {
            using (var image = Image.Load(source, out IImageFormat format))
            {
                var oldRatio = (float)image.Height / image.Width;
                var newRatio = (float)height / width;
                var cropWidth = image.Width;
                var cropHeight = image.Height;

                if (newRatio < oldRatio)
                {
                    // We making the image lower
                    cropHeight = (int)Math.Round(image.Width * newRatio);
                }
                else
                {
                    // We're making the image thinner
                    cropWidth = (int)Math.Round(image.Height / newRatio);
                }

                image.Mutate(x => x.Crop(new Rectangle
                {
                    Width = cropWidth,
                    Height = cropHeight,
                    X = cropWidth < image.Width ? (image.Width - cropWidth) / 2 : 0,
                    Y = cropHeight < image.Height ? (image.Height - cropHeight) / 2 : 0
                }));
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                image.Save(dest, format);
            }
        }
    }
}