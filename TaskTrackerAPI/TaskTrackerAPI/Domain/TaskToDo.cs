﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Domain
{
    public class TaskToDo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
