namespace Raifhack.SkepticalBeavers.Server.Contracts
{
    public static class Routes
    {
        public const string Login = "/login";

        public const string Accounts = "/accounts";

        public const string Restaurants = "/restaurants";

        public static class RestaurantWithId
        {
            public const string Route = "/restaurants/{id:guid}";

            public static class ParamNames
            {
                public const string Id = "id";
            }
        }

        public const string Menus = "/menus";

        public static class MenuWithId
        {
            public const string Route = "/menus/{id:guid}";

            public static class ParamNames
            {
                public const string Id = "id";
            }
        }
    }
}