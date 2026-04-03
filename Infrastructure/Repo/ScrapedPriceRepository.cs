using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;

namespace Infrastructure.Repo
{
    public class ScrapedPriceRepository : Repository<ScrapedPrice>, IScrapedPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public ScrapedPriceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ScrapedPrice>> GetPriceHistoryByProductIdAsync(int productId, int take = 10)
        {
            return await _context.Set<ScrapedPrice>()
                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .OrderByDescending(x => x.ScrapedDateTime)
                .Take(take)
                .ToListAsync();
        }

        public async Task<ScrapedPrice> GetLatestPriceByProductIdAsync(int productId)
        {
            return await _context.Set<ScrapedPrice>()
                .Where(x => x.ProductId == productId && !x.IsDeleted && x.IsSuccessful)
                .OrderByDescending(x => x.ScrapedDateTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url)
        {
            return await _context.Set<ScrapedPrice>()
                .Where(x => x.SourceUrl == url && !x.IsDeleted)
                .OrderByDescending(x => x.ScrapedDateTime)
                .ToListAsync();
        }

        public async Task<List<ScrapedPrice>> GetLatestPricesForProductIdsAsync(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return new List<ScrapedPrice>();

            return await _context.ScrapedPrice
                .Where(sp => productIds.Contains(sp.ProductId)
                          && !sp.IsDeleted
                          && sp.IsSuccessful)
                .GroupBy(sp => sp.ProductId)
                .Select(group => group
                    .OrderByDescending(x => x.ScrapedDateTime)
                    .FirstOrDefault())
                .ToListAsync();
        }
    }
}
