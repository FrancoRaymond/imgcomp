using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string inputFolder = Path.Combine(userProfile, "Desktop", "assets");
        
        string outputFolder = Path.Combine(userProfile, "Desktop", "compressed");

        Directory.CreateDirectory(inputFolder);
        Directory.CreateDirectory(outputFolder);

        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
        var imageFiles = Directory.GetFiles(inputFolder)
                                  .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()))
                                  .ToArray();

        if (imageFiles.Length == 0)
        {
            Console.WriteLine($"No images found in '{inputFolder}'. Drop your images there and run again.");
            return;
        }

        Console.WriteLine($"Found {imageFiles.Length} image(s) in Desktop/assets. Processing...");

        await Parallel.ForEachAsync(imageFiles, async (filePath, token) =>
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string outputPath = Path.Combine(outputFolder, $"{fileName}_compressed.jpg");

            await Task.Run(() => ImageCompressor.CompressToTargetSize(filePath, outputPath, targetSizeKb: 300, maxDimension: 1920));

            FileInfo orig = new FileInfo(filePath);
            FileInfo comp = new FileInfo(outputPath);

            Console.WriteLine($"[Done] {Path.GetFileName(filePath)} ({orig.Length / 1024} KB -> {comp.Length / 1024} KB)");
        });

        Console.WriteLine($"\nAll images compressed! Check the output folder at:\n{outputFolder}");
    }
}
//dotnet run