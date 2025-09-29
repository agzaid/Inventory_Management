namespace Inventory_Management.Models
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RateLimitAttribute : Attribute
    {
        public int MaxRequests { get; }
        public TimeSpan TimeWindow { get; }

        public RateLimitAttribute(int maxRequests, int seconds)
        {
            MaxRequests = maxRequests;
            TimeWindow = TimeSpan.FromSeconds(seconds);
        }
    }
}
