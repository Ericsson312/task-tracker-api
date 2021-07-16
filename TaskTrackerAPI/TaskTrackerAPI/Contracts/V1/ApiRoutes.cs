namespace TaskTrackerApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Boards
        {
            public const string GetAll = Base + "/boards";
            public const string Get = Base + "/boards/{boardId}";
            public const string Update = Base + "/boards/{boardId}";
            public const string Create = Base + "/boards";
            public const string Delete = Base + "/boards/{boardId}";
            
        }
        public static class Cards
        {
            public const string GetAll = Base + "/cards";
            public const string Get = Base + "/cards/{cardId}";
            public const string Update = Base + "/cards/{cardId}";
            public const string Create = Base + "/cards";
            public const string Delete = Base + "/cards/{cardId}";
        }

        public static class Tags
        {
            public const string GetAll = Base + "/tags";
            public const string Get = Base + "/tags/{tagName}";
            public const string Create = Base + "/tags";
            public const string Delete = Base + "/tags/{tagName}";
        }
        
        public static class Members
        {
            public const string GetAll = Base + "/members";
            public const string Get = Base + "/members/{email}";
            public const string Create = Base + "/members/{boardId}";
            public const string Delete = Base + "/members/{boardId}";
            
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
        }
    }
}
