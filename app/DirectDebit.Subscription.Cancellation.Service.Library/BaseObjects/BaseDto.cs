namespace DirectDebit.Subscription.Cancellation.Service.Library.BaseObjects
{
    /// <summary>
    /// Base entity that is to inherited by all Dtos so they all have an Id property of type TKey
    /// </summary>
    /// <typeparam name="TKey">The struct property of the Dto</typeparam>
    public class BaseDto<TKey>
        where TKey : struct
    {
        public TKey DtoId { get; set; }
    }
}
