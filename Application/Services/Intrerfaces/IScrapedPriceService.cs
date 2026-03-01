using Domain.Entities;

namespace Application.Services.Intrerfaces
{
    public interface IScrapedPriceService
    {
        Task<ScrapedPrice> ScrapePriceAsync(int productId, string sourceUrl);
        Task<List<ScrapedPrice>> GetPriceHistoryAsync(int productId, int take = 10);
        Task<ScrapedPrice> GetLatestPriceAsync(int productId);
        Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url);
    }
}
