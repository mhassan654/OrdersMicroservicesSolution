﻿using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.Entities
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string _d { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid ProductId { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        public int Quantity { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
        public decimal UnitPrice { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
        public decimal TotalPrice { get; set; }
    }
}