using MongoDB.Driver;
using BiddingService.Models;
using BiddingService.Repositories.DBContext;

namespace BiddingService.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        private readonly ILogger<BiddingRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<BiddingDTO> _db;

        public BiddingRepository(ILogger<BiddingRepository> logger, IConfiguration configuration, MongoDBContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db.GetCollection<BiddingDTO>("Bids");
        }
        public void AddBid(BiddingDTO bid)
        {
            _logger.LogInformation($"Adding bid id: {bid.BidId} for user id: {bid.UserId}, to auction id: {bid.AuctionId}, with price: {bid.Price}");
            _db.InsertOne(bid);
        }
    }
}
