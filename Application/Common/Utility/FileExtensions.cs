using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
                        Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                        // You can optionally return an error message or handle this differently.
                    }
                }
            }
            // Return the list of uploaded file paths
            return uploadedFilePaths;
        }
    }
}
