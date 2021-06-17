using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts.V1.Requiests
{
    public class CreateTaskRequest
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
    }
}
