using System;
using System.Collections.Generic;

namespace TaskTrackerApi.Contracts.V1.Responses
{
    public class BoardResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public IEnumerable<CardResponse> Cards { get; set; }
    }
}