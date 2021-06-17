using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Contracts
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;
        public static class Tasks
        {
            public const string GetAll = Base + "/tasks";
            public const string Create = "api/v1/tasks";
            public const string Get = "api/v1/tasks/{taskId}";
        }
    }
}
