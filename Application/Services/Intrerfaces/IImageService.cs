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
        IEnumerable<ImageVM> GetAllImages();
        ImageVM GetImageById(int id);
        string CreateImage(ImageVM category);
        bool UpdateImage(ImageVM category);
        bool DeleteImage(int id);
    }
}
