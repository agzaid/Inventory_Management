using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IFeedbackService
    {
        IEnumerable<FeedbackVM> GetAllFeedbacks();
        FeedbackVM GetFeedbackById(int id);
        Task<Result<string>> CreateFeedback(FeedbackVM category);
        Task<bool> UpdateFeedback(FeedbackVM category);
        Task<bool> DeleteFeedback(int id);
        Task<PaginatedResult<FeedbackVM>> GetFeedbackPaginated(int pageNumber, int pageSide);
    }
}
