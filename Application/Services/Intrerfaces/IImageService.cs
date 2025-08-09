using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IImageService
    {
        ImageVM GetImageById(int id);
        Task<IEnumerable<ImageVM>> GetAllImages();
        Task<string> CreateImage(ImageVM obj);
        Task<bool> UpdateImage(ImageVM obj);
        Task<bool> DeleteImage(int id);
    }
}
