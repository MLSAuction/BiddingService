using BiddingService.Models;

namespace BiddingService.Repositories
{
    public interface IBiddingRepository
    {
        void AddBid(BiddingDTO bidding);
    }
}
