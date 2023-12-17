using BiddingService.Models;
using NLog.Targets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using BiddingService.Repositories;
using Microsoft.AspNetCore.DataProtection;
using VaultSharp.V1.Commons;

namespace BiddingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBiddingRepository _repository;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IBiddingRepository repository)
        {
            _logger = logger;
            _configuration = configuration;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("MqHost") };
            using var connection = factory.CreateConnection();

            using var bidChannel = connection.CreateModel();

            bidChannel.QueueDeclare(queue: "bids",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var bidConsumer = new EventingBasicConsumer(bidChannel);
            bidConsumer.Received += (model, ea) =>
            {
                _logger.LogInformation("Bid received, entering bid placement flow");

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                BiddingDTO bid = JsonSerializer.Deserialize<BiddingDTO>(message);

                _repository.AddBid(bid);
            };

            bidChannel.BasicConsume(queue: "bids",
                                    autoAck: true,
                                    consumer: bidConsumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(200000, stoppingToken);
            }
        }
    }
}