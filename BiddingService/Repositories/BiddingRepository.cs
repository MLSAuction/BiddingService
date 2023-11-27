namespace BiddingService.Repositories
{
    public class BiddingRepository
    {
        private readonly ILogger<BiddingRepository> _logger;
        private readonly IConfiguration _configuration;

        public BiddingRepository (ILogger<BiddingRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
    }
}
