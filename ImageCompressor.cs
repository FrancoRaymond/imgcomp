using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

public class ImageCompressor
{
    public static void CompressToTargetSize(string inputPath, string outputPath, int targetSizeKb, int maxDimension = 1920)
    {
        long targetSizeBytes = targetSizeKb * 1024L;

        using (Image image = Image.Load(inputPath))
        {
           
            if (image.Width > maxDimension || image.Height > maxDimension)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxDimension, maxDimension)
                }));
            }

            int quality = 85;
            int minQuality = 20;

            using (MemoryStream ms = new MemoryStream())
            {
                while (quality >= minQuality)
                {
                    ms.SetLength(0);

                    var encoder = new JpegEncoder
                    {
                        Quality = quality
                    };

                    image.Save(ms, encoder);

                    if (ms.Length <= targetSizeBytes)
                    {
                        break;
                    }

                    quality -= 10;
                }

                File.WriteAllBytes(outputPath, ms.ToArray());
            }
        }
    }
}