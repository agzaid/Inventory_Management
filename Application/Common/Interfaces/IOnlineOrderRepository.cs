using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IOnlineOrderRepository : IRepository<OnlineOrder>
    {
        void Update(OnlineOrder onlineOrder);
    }
}
