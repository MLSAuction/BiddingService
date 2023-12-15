using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace BiddingService.Models
{
    public class BiddingDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // This attribute specifies that the Guid should be stored as a string
        public Guid BidId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid AuctionId { get; set; }
        public int Price { get; set; }
        public DateTime TimePlaced { get; set; }
    }
}
