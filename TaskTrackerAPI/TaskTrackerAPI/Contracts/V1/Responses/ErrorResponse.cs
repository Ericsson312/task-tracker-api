using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts.V1.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
                
        }

        public ErrorResponse(ErrorModel errorModel)
        {
                Errors.Add(errorModel);
        }
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
