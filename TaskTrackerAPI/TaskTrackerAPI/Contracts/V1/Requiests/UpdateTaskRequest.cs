using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts.V1.Requiests
{
    public class UpdateTaskRequest
    {
        public string TaskName { get; set; }
    }
}
