using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.V1.Responses
{
    public class BoardResponseExample : IExamplesProvider<BoardResponse>
    {
        public BoardResponse GetExamples()
        {
            return new BoardResponse
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Name of your project",
                Description = "Description for your project",
                Cards = new List<CardResponse>
                {
                    new CardResponse
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid().ToString(),
                        Name = "Card/Task name",
                        Tags = new List<TagResponse>
                        {
                            new TagResponse
                            {
                                Name = "Your fancy tag name"
                            }
                        }
                    }
                },
                Members = new List<MemberResponse>
                {
                    new MemberResponse
                    {
                        Email = "member@example.com"
                    }
                }
            };
        }
    }
}