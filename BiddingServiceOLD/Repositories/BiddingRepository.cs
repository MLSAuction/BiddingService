using Microsoft.AspNetCore.Mvc.Formatters;
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
            _db = db.GetCollection<BiddingDTO>("Bidding"); //Fortæller at vores added-informationer(fx. nye bids) kommer inde under Collection "Bidding" på Mongo

        }

        public IEnumerable<BiddingDTO> GetAllBid()
        {
            return _db.Find(_ => true).ToList();
        }

        public BiddingDTO GetBid(int id)
        {
            // Use MongoDB's LINQ methods to query for a bid by ID
            return _db.Find(u => u.BidId == id).FirstOrDefault();
        }

        public void AddBid(BiddingDTO bidding)
        {
            // Insert a new bid document into the collection
            _db.InsertOne(bidding);
        }

        public BiddingDTO GetHighestBid()
        {
            // Use MongoDB's LINQ methods to query for the bid with the highest price
            return _db.Find(_ => true).SortByDescending(u => u.Price).FirstOrDefault();
        }

        public BiddingDTO GetHighestBidForAuction(int auctionId)
        {
            // Use MongoDB's LINQ methods to query for the highest bid for the specified auctionId
            return _db.Find(u => u.AuctionId == auctionId)
                      .SortByDescending(u => u.Price)
                      .ThenBy(u => u.TimePlaced)
                      .FirstOrDefault();
        }



    }
}
