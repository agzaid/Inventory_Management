using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;


namespace Infrastructure.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _filePath;

        public JsonStringLocalizerFactory(string filePath)
        {
            _filePath = filePath;
        }

        public IStringLocalizer Create(Type resourceSource)
            => (IStringLocalizer)new JsonStringLocalizer(_filePath);

        public IStringLocalizer Create(string baseName, string location)
            => (IStringLocalizer)new JsonStringLocalizer(_filePath);
    }


}
