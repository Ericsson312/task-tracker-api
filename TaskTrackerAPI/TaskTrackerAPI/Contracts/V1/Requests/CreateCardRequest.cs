using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts.V1.Requests
{
    public class CreateCardRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
