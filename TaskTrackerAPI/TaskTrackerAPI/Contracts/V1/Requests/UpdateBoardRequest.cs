using System.Collections.Generic;

namespace TaskTrackerApi.Contracts.V1.Requests
{
    public class UpdateBoardRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}