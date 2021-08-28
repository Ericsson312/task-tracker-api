using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.V1.Responses
{
    public class CardResponseExample : IExamplesProvider<CardResponse>
    {
        public CardResponse GetExamples()
        {
            return new CardResponse()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Card name",
                Tags = new List<TagResponse>
                {
                    new TagResponse
                    {
                        Name = "Tag name"
                    }
                }
            };
        }
    }
}