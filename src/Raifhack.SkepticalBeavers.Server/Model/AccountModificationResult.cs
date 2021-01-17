namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal enum AccountModificationResult
    {
        Success,
        RestaurantNotFound,
        MenuNotFound,
        ItemNotFound,
        ItemKeyAlreadyExists,
        // the following are reserved for weird event duplication scenarios
        AccountAlreadyExists,
        RestaurantAlreadyExists,
        MenuAlreadyExists
    }
}