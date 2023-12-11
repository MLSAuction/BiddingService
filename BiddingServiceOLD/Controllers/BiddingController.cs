using Microsoft.AspNetCore.Mvc;
using BiddingService.Repositories;
using BiddingService.Models;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BiddingController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IBiddingRepository _biddingService;


        public BiddingController(ILogger<BiddingController> logger, IConfiguration configuration, IBiddingRepository biddingRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _biddingService = biddingRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetBid(int id)
        {

            BiddingDTO bidding = _biddingService.GetBid(id);

            if (bidding == null)
            {
                return NotFound(); // Return 404 if bid is not found
            }

            _logger.LogInformation($"{bidding.BidId}, {bidding.Price} - Retrived ");

            return Ok(bidding);
        }

        [HttpPost]
        public IActionResult AddBid([FromBody] BiddingDTO bidding)
        {
            if (bidding == null)
            {
                //If NO "Whole-data". Example: If no texting data in the JSON. 
                return BadRequest("Invalid bidding data");
            }

            if (bidding.BidId == null)
            {
                //Check if there is ID 
                bidding.BidId = GenerateUniqueId();
            }

            if (_biddingService.GetBid((int)bidding.BidId) != null)
            {
                // Handle the case where the ID already exists (e.g., generate a new ID, so it doesnt match the already exist)
                bidding.BidId = GenerateUniqueId();
            }

            _biddingService.AddBid(bidding);

            return CreatedAtAction(nameof(GetBid), new { id = bidding.BidId }, bidding);

        }

        [HttpGet("HighestBid")] //Global
        public IActionResult GetHighestBid()
        {
            // Call a method in your repository to get the highest bid
            BiddingDTO highestBid = _biddingService.GetHighestBid();

            if (highestBid == null)
            {
                return NotFound(); // Return 404 if no bids are found
            }

            _logger.LogInformation($"{highestBid.BidId}, {highestBid.Price} - Highest Bid Retrived ");

            return Ok(highestBid);
        }

        [HttpGet("highest-bid/{auctionId}")] //Highest bid for auctionId
        public IActionResult GetHighestBidForAuction(int auctionId)
        {
            // Call a method in your repository to get the highest bid for the specified auctionId
            BiddingDTO highestBid = _biddingService.GetHighestBidForAuction(auctionId);

            if (highestBid == null)
            {
                return NotFound(); // Return 404 if no bids are found for the specified auctionId
            }

            _logger.LogInformation($"{highestBid.BidId}, {highestBid.Price} - Highest Bid for Auction {auctionId} Retrived ");

            return Ok(highestBid);
        }




        private int GenerateUniqueId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }


}
