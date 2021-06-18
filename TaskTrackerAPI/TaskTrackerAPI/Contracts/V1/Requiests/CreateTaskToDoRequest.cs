using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts.V1.Requiests
{
    public class CreateTaskToDoRequest
    {
        public string Name { get; set; }
    }
}
