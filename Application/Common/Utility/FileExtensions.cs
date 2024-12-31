using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Drawing;

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
        public static async Task<List<string>> CreateImages(List<IFormFile> images, string name)
        {
            try
            {
                // Check if there are images in the list
                if (images == null || images.Count == 0)
                {
                    return new List<string>();  // Return an empty list if no images were provided
                }

                var uploadedFilePaths = new List<string>();

                // Directory to save files (path includes the provided name)
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", name);

                // Create the directory if it doesn't exist
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Iterate over each file and save it
                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique file name to avoid overwriting
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);

                        // Add a timestamp or GUID to make the filename unique
                        string uniqueFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
                        string filePath = Path.Combine(uploadDirectory, uniqueFileName);

                        try
                        {
                            // Save the file to the specified path
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                // Add the relative file path to the list
                                uploadedFilePaths.Add($"/UploadedFiles/{name}/{uniqueFileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error (you can use a logger or simply print it)
                            uploadedFilePaths.Add($"Error uploading file {file.FileName}: {ex.Message}");
                            // You can optionally return an error message or handle this differently.
                        }
                    }
                }
                // Return the list of uploaded file paths
                return uploadedFilePaths;
            }
            catch (Exception ex)
            {

                throw;
            }
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
        public static Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new(byteArray))
            {
                return Image.FromStream(ms);
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
