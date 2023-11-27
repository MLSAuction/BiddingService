using Microsoft.AspNetCore.Mvc;
using BiddingService.Repositories;

namespace BiddingService.Controllers
{
    public class BiddingController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly BiddingRepository _repository;

        BiddingController (ILogger logger, IConfiguration configuration, BiddingRepository repository)
        {
            _logger = logger;
            _configuration = configuration;
            _repository = repository;
        }
    }
}
