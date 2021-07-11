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

        public static class Cards
        {
            public const string GetAll = Base + "/cards";
            public const string Get = Base + "/cards/{cardId}";
            public const string Create = Base + "/cards";
            public const string Update = Base + "/cards/{cardId}";
            public const string Delete = Base + "/cards/{cardId}";
        }

        public static class Tags
        {
            public const string GetAll = Base + "/tags";
            public const string Get = Base + "/tags/{tagName}";
            public const string Create = Base + "/tags";
            public const string Delete = Base + "/tags/{tagName}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
        }
    }
}
