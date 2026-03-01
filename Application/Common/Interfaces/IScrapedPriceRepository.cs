using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IScrapedPriceRepository : IRepository<ScrapedPrice>
    {
        Task<List<ScrapedPrice>> GetPriceHistoryByProductIdAsync(int productId, int take = 10);
        Task<ScrapedPrice> GetLatestPriceByProductIdAsync(int productId);
        Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url);
    }
}
