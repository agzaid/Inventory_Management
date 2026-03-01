using Application.Services.Intrerfaces;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Services.Implementation
{
    public class ScrapedPriceService : IScrapedPriceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScrapedPriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ScrapedPrice> ScrapePriceAsync(int productId, string sourceUrl)
        {
            // For now, this will create a stub record
            // The actual scraping will be done through a separate endpoint
            var scrapedPrice = new ScrapedPrice
            {
                ProductId = productId,
                SourceUrl = sourceUrl,
                Price = 0,
                Currency = "EGP",
                ScraperMethod = "Pending",
                IsSuccessful = false,
                ErrorMessage = "Scraping not yet executed",
                ScrapedDateTime = DateTime.UtcNow
            };

            await _unitOfWork.ScrapedPrice.AddAsync(scrapedPrice);
            await _unitOfWork.SaveAsync();

            return scrapedPrice;
        }

        public async Task<List<ScrapedPrice>> GetPriceHistoryAsync(int productId, int take = 10)
        {
            return await _unitOfWork.ScrapedPrice.GetPriceHistoryByProductIdAsync(productId, take);
        }

        public async Task<ScrapedPrice> GetLatestPriceAsync(int productId)
        {
            return await _unitOfWork.ScrapedPrice.GetLatestPriceByProductIdAsync(productId);
        }

        public async Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url)
        {
            return await _unitOfWork.ScrapedPrice.GetPricesByUrlAsync(url);
        }
    }
}
