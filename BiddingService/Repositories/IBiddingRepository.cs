using BiddingService.Models;

namespace BiddingService.Repositories
{
    public interface IBiddingRepository
    {
        IEnumerable<BiddingDTO> GetAllBid();
        BiddingDTO GetBid(int id);
        void AddBid(BiddingDTO bidding);
        BiddingDTO GetHighestBid();
        BiddingDTO GetHighestBidForAuction(int auctionId);


    }
}
