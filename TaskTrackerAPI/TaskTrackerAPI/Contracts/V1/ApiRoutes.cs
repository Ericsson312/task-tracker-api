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
            public const string Get = Base + "/tasks/{taskId}";
            public const string Update = Base + "/tasks/{taskId}";
            public const string Delete = Base + "/tasks/{taskId}";
            public const string Create = Base + "/tasks";
        }

        public static class Tags
        {
            public const string GetAll = Base + "/tags";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
        }
    }
}
