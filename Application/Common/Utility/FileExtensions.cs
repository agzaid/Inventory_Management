using Domain.Enums;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Application.Common.Utility
{
    public static class FileExtensions
    {
        public static async Task<string> CreateFile(this IFormFile file, string userDirectory)
        {
            try
            {
                if (file.Length > 0)
                {
                    if (file.Length > 5 * 1024 * 1024) // 5 MB limit
                    {
                        throw new InvalidOperationException("File size must be less than 5 MB.");
                    }
                    var img = Guid.NewGuid();
                    var directoryPath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\" + userDirectory;
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using var stream = new FileStream($"{directoryPath}/{img}{Path.GetExtension(file.FileName)}", FileMode.Create);

                    await file.CopyToAsync(stream);

                    return $"{img}{Path.GetExtension(file.FileName)}";
                }
                return string.Empty;

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static async Task<List<string>> CreateFolderFile(this List<IFormFile> files, string userDirectory, string userMRN, DateTime wantedDate)
        {
            try
            {
                List<string> images = new List<string>();
                //List<string> uploadedFiles = new List<string>(); // Track uploaded files
                var date = wantedDate.ToShortDateString();
                var changedDate = date.Replace('/', ',');
                var directoryPath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\" + userMRN;
                var directoryPath2 = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\" + userMRN + "\\" + userDirectory + changedDate;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                if (!Directory.Exists(directoryPath2))
                {
                    Directory.CreateDirectory(directoryPath2);
                }
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files[i].Length > 5 * 1024 * 1024) // 5 MB limit
                    {
                        throw new InvalidOperationException("File size must be less than 5 MB.");
                    }
                    //using var stream = new FileStream($"{directoryPath2}/{files[i].FileName}{Path.GetExtension(files[i].FileName)}", FileMode.Create);
                    using var stream = new FileStream($"{directoryPath2}/{files[i].FileName}", FileMode.Create);
                    await files[i].CopyToAsync(stream);

                    var path = $"{directoryPath2}\\{files[i].FileName}";
                    var splitted = path.Split("Uploads");
                    images.Add(splitted[1]);
                    //uploadedFiles.Add(directoryPath2); // Track the uploaded file path
                }
                return images;
            }
            catch (Exception)
            {
                return new List<string> { "Error" };
                throw;
            }
        }
        public static async Task<string> SaveImageOptimized(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid image file");

            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Generate unique file name
            string fileName = $"{Guid.NewGuid()}.webp";
            string filePath = Path.Combine(uploadDir, fileName);

            using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());

            // Resize if bigger than 1200px wide
            if (image.Width > 1200)
            {
                image.Mutate(x => x.Resize(1200, 0)); // keep aspect ratio
            }

            // Save as WebP (smaller & faster than JPG/PNG)
            await image.SaveAsWebpAsync(filePath);

            // return relative path for later use
            return $"/{folder}/{fileName}";
        }
        public static async Task<List<string>> SaveImagesOptimized(List<IFormFile> files, string folder)
        {
            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                string savedPath = await SaveImageOptimized(file, folder);
                uploadedFiles.Add(savedPath);
            }

            return uploadedFiles;
        }



        public static async Task DeleteImages(List<string> images)
        {
            foreach (var imagePath in images)
            {
                try
                {
                    var image = "wwwroot" + imagePath;
                    
                    if (File.Exists(image))
                    {
                        await Task.Run(() => File.Delete(image));
                        Console.WriteLine($"Image at {image} deleted successfully.");

                        if (Directory.Exists(image))
                        {
                            // Get all files in the directory (including files in subdirectories, if needed)
                            string[] files = Directory.GetFiles(image);

                            // Get the count of files
                            int fileCount = files.Length;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Image at {imagePath} does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting image: {ex.Message}");
                }
            }

        }
        public static async Task DeleteEmptyFolder(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    await Task.Run(() => Directory.Delete(folderPath));
                    Console.WriteLine($"Folder '{folderPath}' has been deleted.");
                }
                else
                {
                    Console.WriteLine($"Folder '{folderPath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static byte[] ConvertImageToByteArray(IFormFile imageFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Copy the content of the image file into the memory stream
                imageFile.CopyTo(memoryStream);

                // Convert the memory stream to a byte array
                return memoryStream.ToArray();
            }
        }
        public static byte[] ConvertImageToByteArray(IFormFile imageFile, int targetWidth, long quality)
        {
            using (var inputStream = imageFile.OpenReadStream())
            using (var image = System.Drawing.Image.FromStream(inputStream))
            {
                int targetHeight = (int)(image.Height * ((float)targetWidth / image.Width));

                using (var resizedImage = new Bitmap(targetWidth, targetHeight))
                {
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                    }

                    using (var ms = new MemoryStream())
                    {
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                        var jpegCodec = ImageCodecInfo.GetImageDecoders()
                            .First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                        resizedImage.Save(ms, jpegCodec, encoderParams);

                        return ms.ToArray();
                    }
                }
            }
        }

        public static System.Drawing.Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new(byteArray))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }
        public static string ByteArrayToImageBase64(byte[] byteArray)
        {
            string base64Image = Convert.ToBase64String(byteArray);
            string imageSrc = $"data:image/jpeg;base64,{base64Image}";
            return imageSrc;
        }
        public static byte[] FromImageToByteArray(string array)
        {
            string base64String = array;

            // Step 1: Check if the string contains the Base64 prefix and remove it
            if (base64String.Contains("data:image") && base64String.Contains("base64,"))
            {
                base64String = base64String.Split(',')[1];  // Remove the prefix
            }

            // Step 2: Ensure the Base64 string length is a multiple of 4
            int mod4 = base64String.Length % 4;
            if (mod4 > 0)
            {
                base64String = base64String.PadRight(base64String.Length + (4 - mod4), '=');
            }
            byte[] imageBytes = Convert.FromBase64String(base64String);
            return imageBytes;
        }

    }
}
